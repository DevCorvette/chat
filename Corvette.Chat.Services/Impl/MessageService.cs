using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Corvette.Chat.Data;
using Corvette.Chat.Data.Entities;
using Corvette.Chat.Services.Extensions;
using Corvette.Chat.Services.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Corvette.Chat.Services.Impl
{
    public class MessageService : IMessageService
    {
        private readonly ILogger<MessageService> _logger;

        private readonly IChatDataContextFactory _contextFactory;

        private readonly IMemberService _memberService;

        public MessageService(
            ILogger<MessageService> logger, 
            IChatDataContextFactory contextFactory, 
            IMemberService memberService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<MessageForRecipient>> AddMessageAsync(UserModel author, Guid chatId, string text)
        {
            _logger.LogDebug($"{nameof(AddMessageAsync)} started by author id: {author.Id}, chatId: {chatId}, text: {text.Substring(0, 10)}...");
            await using var context = _contextFactory.CreateContext();

            // checks
            if (author == null) throw new ArgumentNullException(nameof(author));
            if (chatId == default) throw new ArgumentOutOfRangeException(nameof(chatId));
            if (!text.HasValue()) throw new ArgumentNullException(nameof(text));
            await _memberService.ThrowIfAccessDenied(context, author.Id, chatId);

            // add message
            var message = new MessageEntity
            {
                AuthorId = author.Id,
                ChatId = chatId,
                Text = text
            };

            context.Messages.Add(message);
            await context.SaveChangesAsync();
            
            _logger.LogDebug($"{nameof(AddMessageAsync)} successfully added message id: {message.Id} by authorId: {author.Id}, chatId: {chatId}");

            // count unread for all recipients
            var recipients = await (from cu in context.ChatUsers
                    join m in context.Messages on cu.ChatId equals m.ChatId
                    where cu.ChatId == chatId
                    where cu.UserId != author.Id
                    where m.Created > cu.LastReadDate
                    group cu by cu.ChatId into gr
                    select gr)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
            
            _logger.LogDebug($"{nameof(AddMessageAsync)} successfully count unread messages for {recipients.Count} recipients");

            return recipients
                .Select(x => new MessageForRecipient(message, author.Name, x.Value, x.Key))
                .ToList();
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<MessageModel>> GetLastWithUnreadAsync(UserModel user, Guid chatId, int take)
        {
            await using var context = _contextFactory.CreateContext();

            if (user == null) throw new ArgumentNullException(nameof(user));
            if (take <= 0) throw new ArgumentOutOfRangeException(nameof(take));
            await _memberService.ThrowIfAccessDenied(context, user.Id, chatId);

            // get last read date
            var lastReadDate = await context.ChatUsers
                .Where(x => x.UserId == user.Id)
                .Where(x => x.ChatId == chatId)
                .Select(x => x.LastReadDate)
                .SingleOrDefaultAsync();
            
            var messages = new List<MessageModel>(take);
            
            var query = context.Messages
                .Where(x => x.ChatId == chatId)
                .OrderByDescending(x => x.Created)
                .AsQueryable();

            // get read
            messages.AddRange(await query
                .Where(x => x.Created > lastReadDate) // top half
                .Select(x => new MessageModel(x, x.Author.Name))
                .Take(take/2) 
                .ToListAsync());
            
            // get unred
            messages.AddRange(await query
                .Where(x => x.Created < lastReadDate) // lower half
                .Select(x => new MessageModel(x, x.Author.Name))
                .Take(take - messages.Count)
                .ToListAsync());

            return messages;
        }
        
        /// <inheritdoc/>
        public async Task<IReadOnlyList<MessageModel>> GetMessagesAsync(UserModel user, Guid chatId, DateTime skip, int take, bool isSkipTop)
        {
            await using var context = _contextFactory.CreateContext();

            if (user == null) throw new ArgumentNullException(nameof(user));
            if (take <= 0) throw new ArgumentOutOfRangeException(nameof(take));
            await _memberService.ThrowIfAccessDenied(context, user.Id, chatId);
            
            // get
            var query = context.Messages
                .Where(x => x.ChatId == chatId)
                .OrderByDescending(x => x.Created)
                .AsQueryable();

            query = isSkipTop 
                ? query.Where(x => x.Created > skip)  // skip top messages
                : query.Where(x => x.Created < skip); // skip lower messages

            return await query
                .Select(x => new MessageModel(x, x.Author.Name))
                .Take(take)
                .ToListAsync();
        }
    }
}