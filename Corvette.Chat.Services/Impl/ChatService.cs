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
    public class ChatService : IChatService
    {
        private readonly ILogger<ChatService> _logger;

        private readonly IChatDataContextFactory _contextFactory;

        public ChatService(
            ILogger<ChatService> logger, 
            IChatDataContextFactory contextFactory)
        {
            _logger = logger;
            _contextFactory = contextFactory;
        }
        
        /// <inheritdoc/>
        public async Task<ChatModel> CreateChatAsync(UserModel creator, string? name, IReadOnlyList<Guid> memberIds, bool isPrivate)
        {
            _logger.LogDebug($"{nameof(CreateChatAsync)} started by creator with id: {creator.Id}, chat name: {name}, isPrivate: {isPrivate}");
            await using var context = _contextFactory.CreateContext();

            // checks
            if (creator == null) throw new ArgumentNullException(nameof(creator));
            if (memberIds == null) throw new ArgumentNullException(nameof(memberIds));
            if (memberIds.Count < 1) throw new ArgumentOutOfRangeException(nameof(memberIds), "Can't be empty.");
            if (!name.HasValue() && !isPrivate) throw new ArgumentOutOfRangeException(nameof(name), "Can't be empty or null for public chat.");
            if (memberIds.Count > 1 && isPrivate) throw new ChatServiceException("Can't create a private chat between more than 2 users.");
            
            // get and check members
            var members = await context.Users.AsNoTracking()
                .Where(x => memberIds.Contains(x.Id))
                .ToListAsync();
            
            if (members.Count < 1)
                throw new EntityNotFoundException("Members wasn't found. Can't create a chat without members.");

            // create
            var chat = new ChatEntity
            {
                OwnerId = creator.Id,
                IsPrivate = isPrivate,
                Name = isPrivate ? null : name,
                ChatUsers = members
                    .Select(x => new ChatUserEntity {UserId = x.Id})
                    .ToList(),
            };

            context.Add(chat);
            await context.SaveChangesAsync();

            // result
            var model = new ChatModel(
                chat, 
                isPrivate ? members.First().Name : chat.Name, 
                null, 
                0);

            _logger.LogInformation($"{nameof(CreateChatAsync)} successfully created new chat: {model}");
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
                    select m)
                .Include(m => m.Author)
                .GroupBy(x => x.ChatId)
                .Select(x => x.First())
                .ToDictionaryAsync(x => x.ChatId, x => x);

            // count unread
            var countDic = await (from cu in context.ChatUsers
                    join m in context.Messages on cu.ChatId equals m.ChatId
                    where cu.UserId == userId
                    where m.Created > cu.LastReadDate
                    select m)
                .GroupBy(x => x.ChatId)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
                
            // convert
            var models = allChats.ConvertAll(x => new ChatModel(
                x,
                x.IsPrivate ? privateNames[x.Id] : x.Name,
                new MessageModel(messagesDic[x.Id]), 
                countDic[x.Id]));
            
            return models;
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
        public async Task RemoveAsync(UserModel owner, Guid chatId)
        {
            _logger.LogDebug($"{nameof(RemoveAsync)} started by user with id: {owner.Id} for chat id: {chatId}");
            await using var context = _contextFactory.CreateContext();

            // checks
            if (owner == null) throw new ArgumentNullException(nameof(owner));
            if (chatId == default) throw new ArgumentOutOfRangeException(nameof(chatId));
            
            var chat = await context.Chats
                           .SingleOrDefaultAsync(x => x.Id == chatId)
                       ?? throw new EntityNotFoundException($"Chat by id: {chatId} was not found.");
            
            if (chat.OwnerId != owner.Id) throw new ForbiddenException("Can't remove the chat because the current user is not the owner.");

            context.Remove(chat);
            await context.SaveChangesAsync();

            _logger.LogInformation($"{nameof(RemoveAsync)} successfully remove chat with id: {chatId}");
        }
    }
}