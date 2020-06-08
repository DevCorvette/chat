using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Corvette.Chat.Data;
using Corvette.Chat.Data.Entities;
using Corvette.Chat.Services.Exceptions;
using Corvette.Chat.Services.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Corvette.Chat.Services.Impl
{
    public class ChatUserService : IChatUserService
    {
        private readonly ILogger<ChatUserService> _logger;
        
        private readonly IChatDataContextFactory _contextFactory;

        public ChatUserService(ILogger<ChatUserService> logger, IChatDataContextFactory contextFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        /// <inheritdoc/>
        public async Task ThrowIfAccessDenied(ChatDataContext context, Guid userId, Guid chatId)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var isUserInChat = await context.ChatUsers
                .Where(x => x.UserId == userId)
                .Where(x => x.ChatId == chatId)
                .AnyAsync();
            
            if(!isUserInChat)
                throw new ForbiddenException($"User with id: {userId} doesn't have access to chat with id: {chatId}");
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<UserModel>> GetMembersAsync(UserModel user, Guid chatId)
        {
            await using var context = _contextFactory.CreateContext();
            
            // checks
            if (user == null) throw new ArgumentNullException(nameof(user));
            await ThrowIfAccessDenied(context, user.Id, chatId);

            // get
            return await context.ChatUsers
                .Where(x => x.ChatId == chatId)
                .Select(x => new UserModel(x.User))
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task AddMembersAsync(UserModel owner, Guid chatId, IReadOnlyList<Guid> newMemberIds)
        {
            await using var context = _contextFactory.CreateContext();
            
            // checks
            if (owner == null) throw new ArgumentNullException(nameof(owner));
            if (newMemberIds == null) throw new ArgumentNullException(nameof(newMemberIds));
            _logger.LogDebug($"{nameof(AddMembersAsync)} started by user with id: {owner.Id} for chat with id: {chatId} and {newMemberIds.Count} new members");

            var chat = await context.Chats
                           .SingleOrDefaultAsync(x => x.Id == chatId)
                       ?? throw new EntityNotFoundException($"Chat by id: {chatId} was not found.");
            
            if (chat.OwnerId != owner.Id) throw new ForbiddenException("Can't add members to the chat because the current user is not the owner.");
            if (chat.IsPrivate) throw new ChatServiceException("Can't add members to the chat because private chat can has only 2 members.");

            // add
            var count = 0;
            foreach (var userId in newMemberIds)
            {
                // is exist?
                var isUserExist = await context.Users
                    .Where(x => x.Id == userId)
                    .AnyAsync();
                if(!isUserExist)
                    throw new EntityNotFoundException($"User with id: {userId} was not found");
                
                // is already added?
                var isAdded = await context.ChatUsers
                    .Where(x => x.UserId == userId)
                    .Where(x => x.ChatId == chatId)
                    .AnyAsync();
                
                if (!isAdded)
                {
                    context.Add(new ChatUserEntity
                    {
                        UserId = userId,
                        ChatId = chatId,
                    });
                    count++;
                }
            }

            await context.SaveChangesAsync();
            _logger.LogInformation($"{nameof(AddMembersAsync)} successfully added {count} new members to chat with id: {chatId}");
        }

        /// <inheritdoc/>
        public async Task LeaveChatAsync(UserModel user, Guid chatId)
        {
            await using var context = _contextFactory.CreateContext();
            
            // checks
            if (user == null) throw new ArgumentNullException(nameof(user));
            _logger.LogDebug($"{nameof(LeaveChatAsync)} started by user with id: {user.Id} for chat with id: {chatId}");

            var chatUser = await context.ChatUsers
                           .Where(x => x.UserId == user.Id)
                           .Where(x => x.ChatId == chatId)
                           .SingleOrDefaultAsync()
                       ?? throw new EntityNotFoundException($"User with id: {user.Id} was not found in a chat with id: {chatId}.");

            // remove
            context.ChatUsers.Remove(chatUser);
            await context.SaveChangesAsync();
            
            _logger.LogInformation($"{nameof(LeaveChatAsync)} successfully finished for user with id: {user.Id} and chat with id: {chatId}");
        }
    }
}