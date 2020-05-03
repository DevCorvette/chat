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

        public MessageService(
            ILogger<MessageService> logger, 
            IChatDataContextFactory contextFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
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

            // count unread
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
        public Task<IReadOnlyList<MessageModel>> GetMessagesAsync(Guid chatId, DateTime? skip, int take)
        {
            throw new NotImplementedException();
        }
    }
}