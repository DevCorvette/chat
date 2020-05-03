using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Corvette.Chat.Data;
using Corvette.Chat.Data.Entities;
using Corvette.Chat.Services.Exceptions;
using Corvette.Chat.Services.Extensions;
using Corvette.Chat.Services.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Corvette.Chat.Services.Impl
{
    /// <inheritdoc/>
    public sealed class ChatService : IChatService
    {
        private readonly ILogger<ChatService> _logger;

        private readonly IChatDataContextFactory _contextFactory;

        public ChatService(
            ILogger<ChatService> logger, 
            IChatDataContextFactory contextFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }
        
        /// <inheritdoc/>
        public async Task<ChatModel> CreatePublicChatAsync(UserModel creator, string name)
        {
            _logger.LogDebug($"{nameof(CreatePublicChatAsync)} started by creator with id: {creator.Id}, chat name: {name}");
            await using var context = _contextFactory.CreateContext();

            // checks
            if (creator == null) throw new ArgumentNullException(nameof(creator));
            if (!name.HasValue()) throw new ArgumentNullException(nameof(name), "Can't be empty or null for public chat.");

            // create
            var chat = new ChatEntity
            {
                OwnerId = creator.Id,
                Name = name,
                IsPrivate = false,
            };

            context.Add(chat);
            await context.SaveChangesAsync();

            // result
            var model = new ChatModel(chat, chat.Name, null);

            _logger.LogInformation($"{nameof(CreatePublicChatAsync)} successfully created new chat: {model}");
            return model;
        }
        
        /// <inheritdoc/>
        public async Task<ChatModel> CreatePrivateChatAsync(UserModel creator, Guid interlocutorId)
        {
            _logger.LogDebug($"{nameof(CreatePrivateChatAsync)} started by creator with id: {creator.Id}, {nameof(interlocutorId)}: {interlocutorId}");
            await using var context = _contextFactory.CreateContext();

            // checks
            if (creator == null) throw new ArgumentNullException(nameof(creator));
            if (interlocutorId == default) throw new ArgumentOutOfRangeException(nameof(interlocutorId));

            var interlocutorName = await context.Users
                                       .Where(x => x.Id == interlocutorId)
                                       .Select(x => x.Name)
                                       .SingleOrDefaultAsync()
                                   ?? throw new EntityNotFoundException($"Interlocutor by id: {interlocutorId} was not found.");

            // create
            var chat = new ChatEntity
            {
                OwnerId = creator.Id,
                IsPrivate = true,
                ChatUsers = new List<ChatUserEntity> {new ChatUserEntity {UserId = interlocutorId}},
            };

            context.Add(chat);
            await context.SaveChangesAsync();

            // result
            var model = new ChatModel(chat, interlocutorName, null);

            _logger.LogInformation($"{nameof(CreatePrivateChatAsync)} successfully created new chat: {model}");
            return model;
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<ChatModel>> GetAllChatsAsync(Guid userId)
        {
            await using var context = _contextFactory.CreateContext();

            // check
            if (userId == default) throw new ArgumentOutOfRangeException(nameof(userId));

            // get all user's chat
            var allChats =await context.ChatUsers
                .Where(x => x.UserId == userId)
                .Select(x => x.Chat)
                .ToListAsync();
            
            // early exit
            if (allChats.Count == 0) return new ChatModel[0];
            
            // find names for private chats
            var privateChatIds = allChats
                .Where(x => x.IsPrivate)
                .Select(x => x.Id)
                .ToList();
            
            var privateNames = await context.ChatUsers
                .Where(x => privateChatIds.Contains(x.ChatId))
                .Where(x => x.UserId != userId)
                .ToDictionaryAsync(
                    x => x.ChatId,
                    x => x.User.Name);
            
            // get last messages
            var messagesDic = await (from cu in context.ChatUsers
                    join m in context.Messages on cu.ChatId equals m.ChatId
                    where cu.UserId == userId
                    orderby m.Created descending
                    group m by m.ChatId into gr
                    select gr.First())
                .Include(m => m.Author)
                .ToDictionaryAsync(mes => mes.ChatId, mes => mes);

            // count unread
            var countDic = await (from cu in context.ChatUsers
                    join m in context.Messages on cu.ChatId equals m.ChatId
                    where cu.UserId == userId
                    where m.Created > cu.LastReadDate
                    group m by m.ChatId into gr
                    select gr)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
                
            // convert
            var models = allChats.ConvertAll(x => new ChatModel(
                x,
                x.IsPrivate ? privateNames[x.Id] : x.Name,
                new MessageModel(messagesDic[x.Id], countDic[x.Id])));
            
            return models;
        }

        /// <inheritdoc/>
        public async Task<ChatModel> GetChatAsync(UserModel user, Guid chatId)
        {
            await using var context = _contextFactory.CreateContext();
            
            // check
            if (chatId == default) throw new ArgumentOutOfRangeException(nameof(chatId));
            if (user == null) throw new ArgumentNullException(nameof(user));
            
            // chat
            var chat = await context.Chats
                           .Where(x => x.Id == chatId)
                           .SingleOrDefaultAsync()
                       ?? throw new EntityNotFoundException($"Chat by id: {chatId} was not found.");

            // name
            var chatName = chat.Name;
            if (chat.IsPrivate)
            {
                chatName = await context.ChatUsers
                               .Where(x => x.ChatId == chatId)
                               .Where(x => x.UserId != user.Id)
                               .Select(x => x.User.Name)
                               .SingleOrDefaultAsync()
                           ?? throw new EntityNotFoundException($"Can't find interlocutor in private chat: {chatId}");
            }
            
            // message
            var message = await context.Messages
                .Where(x => x.ChatId == chatId)
                .OrderByDescending(x => x.Created)
                .Include(x => x.Author)
                .FirstOrDefaultAsync();

            // unread
            var count = 0;
            if (message != null)
            {
                count = await (from cu in context.ChatUsers
                        join m in context.Messages on cu.ChatId equals m.ChatId
                        where cu.UserId == user.Id
                        where m.Created > cu.LastReadDate
                        select m)
                    .CountAsync();
            }
            
            // result
            return new ChatModel(
                chat, 
                chatName, 
                message != null ? new MessageModel(message, count) : null);
        }

        /// <inheritdoc/>
        public async Task RenameChatAsync(UserModel owner, Guid chatId, string name)
        {
            _logger.LogDebug($"{nameof(RenameChatAsync)} started by user with id: {owner.Id} for chatId: {chatId}, name: {name}");
            await using var context = _contextFactory.CreateContext();

            // check
            if (owner == null) throw new ArgumentNullException(nameof(owner));
            if (chatId == default) throw new ArgumentOutOfRangeException(nameof(chatId));
            if (!name.HasValue()) throw new ArgumentOutOfRangeException(nameof(name));
            
            var chat = await context.Chats
                           .SingleOrDefaultAsync(x => x.Id == chatId)
                       ?? throw new EntityNotFoundException($"Chat by id: {chatId} was not found.");
            
            if (chat.OwnerId != owner.Id) throw new ForbiddenException("Can't update the chat because the current user is not the owner.");
            if (chat.IsPrivate) throw new ChatServiceException("Can't change chat name because private chat doesn't have name.");

            // update
            chat.Name = name;
            await context.SaveChangesAsync();
            
            _logger.LogInformation($"{nameof(RenameChatAsync)} successfully renamed chat Id: {chatId}, name: {name}");
        }

        /// <inheritdoc/>
        public async Task ChangeOwnerAsync(UserModel owner, Guid chatId, Guid newOwnerId)
        {
            _logger.LogDebug($"{nameof(ChangeOwnerAsync)} started by user with id: {owner.Id} for chat id: {chatId}, newOwnerId: {newOwnerId}");
            await using var context = _contextFactory.CreateContext();

            // checks
            if (owner == null) throw new ArgumentNullException(nameof(owner));
            if (chatId == default) throw new ArgumentOutOfRangeException(nameof(chatId));
            if (newOwnerId == default) throw new ArgumentOutOfRangeException(nameof(newOwnerId));
            
            if (!await context.Users.AnyAsync(x => x.Id == newOwnerId))
                throw new EntityNotFoundException($"User by id: {newOwnerId} was not found.");
            
            // check chat
            var chat = await context.Chats
                           .SingleOrDefaultAsync(x => x.Id == chatId)
                       ?? throw new EntityNotFoundException($"Chat by id: {chatId} was not found.");

            if (chat.OwnerId != owner.Id) throw new ForbiddenException("Can't update the chat because the current user is not the owner.");
            if (chat.IsPrivate) throw new ChatServiceException("Can't change private chat's owner.");

            // update
            chat.OwnerId = newOwnerId;
            await context.SaveChangesAsync();
            
            _logger.LogInformation($"{nameof(ChangeOwnerAsync)} successfully change owner id: {newOwnerId} for chat id: {chatId}");
        }

        /// <inheritdoc/>
        public async Task RemovePublicAsync(UserModel owner, Guid chatId)
        {
            _logger.LogDebug($"{nameof(RemovePublicAsync)} started by user with id: {owner.Id} for chat id: {chatId}");
            await using var context = _contextFactory.CreateContext();

            // checks
            if (owner == null) throw new ArgumentNullException(nameof(owner));
            if (chatId == default) throw new ArgumentOutOfRangeException(nameof(chatId));
            
            var chat = await context.Chats
                           .SingleOrDefaultAsync(x => x.Id == chatId)
                       ?? throw new EntityNotFoundException($"Chat by id: {chatId} was not found.");
            
            if (chat.OwnerId != owner.Id) throw new ForbiddenException("Can't remove the chat because the current user is not the owner.");
            if (chat.IsPrivate) throw new ChatServiceException("Can't remove private chat.");

            // remove
            context.Remove(chat);
            await context.SaveChangesAsync();

            _logger.LogInformation($"{nameof(RemovePublicAsync)} successfully remove chat with id: {chatId}");
        }
    }
}