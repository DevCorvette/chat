using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Xsl;
using Corvette.Chat.Data;
using Corvette.Chat.Data.Entities;
using Corvette.Chat.Logic.Exceptions;
using Corvette.Chat.Logic.Extensions;
using Corvette.Chat.Logic.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Corvette.Chat.Logic.Impl
{
    /// <inheritdoc/>
    public sealed class ChatService : IChatService
    {
        private readonly ILogger<ChatService> _logger;

        private readonly IChatDataContextFactory _contextFactory;

        private readonly IMemberService _memberService;

        public ChatService(ILogger<ChatService> logger, IChatDataContextFactory contextFactory, IMemberService memberService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
        }
        
        /// <inheritdoc/>
        public async Task<ChatModel> CreatePublicChatAsync(UserModel creator, string name)
        {
            if (creator == null) throw new ArgumentNullException(nameof(creator));
            if (!name.HasValue()) throw new ArgumentNullException(nameof(name), "Can't be empty or null for public chat.");
            
            _logger.LogDebug($"{nameof(CreatePublicChatAsync)} started by creator with id: {creator.Id}, chat name: {name}");
            await using var context = _contextFactory.CreateContext();

            // create
            var chat = new ChatEntity
            {
                OwnerId = creator.Id,
                Name = name,
                IsPrivate = false,
                ChatUsers = new List<MemberEntity> {new MemberEntity {UserId = creator.Id}},
            };

            context.Add(chat);
            await context.SaveChangesAsync();

            // result
            var model = new ChatModel
            {
                Id = chat.Id,
                Created = chat.Created,
                IsPrivate = chat.IsPrivate,
                Name = chat.Name,
                LastMessage = null,
                UnreadCount = 0,
            };
            
            _logger.LogInformation($"{nameof(CreatePublicChatAsync)} successfully created new public chat: {model}");
            return model;
        }
        
        /// <inheritdoc/>
        public async Task<ChatModel> CreatePrivateChatAsync(UserModel creator, Guid interlocutorId)
        {
            if (creator == null) throw new ArgumentNullException(nameof(creator));
            
            _logger.LogDebug($"{nameof(CreatePrivateChatAsync)} started by creator with id: {creator.Id}, {nameof(interlocutorId)}: {interlocutorId}");
            await using var context = _contextFactory.CreateContext();

            // check
            var interlocutorName = await context.Users
                                       .Where(x => x.Id == interlocutorId)
                                       .Select(x => x.Name)
                                       .SingleOrDefaultAsync()
                                   ?? throw new EntityNotFoundException($"Interlocutor with id: {interlocutorId} was not found.");

            // create
            var chat = new ChatEntity
            {
                OwnerId = creator.Id,
                IsPrivate = true,
                ChatUsers = new List<MemberEntity>
                {
                    new MemberEntity {UserId = creator.Id},
                    new MemberEntity {UserId = interlocutorId},
                },
            };

            context.Add(chat);
            await context.SaveChangesAsync();

            // result
            var model = new ChatModel
            {
                Id = chat.Id,
                Created = chat.Created,
                IsPrivate = chat.IsPrivate,
                Name = interlocutorName,
                LastMessage = null,
                UnreadCount = 0,
            };

            _logger.LogInformation($"{nameof(CreatePrivateChatAsync)} successfully created new private chat: {model}");
            return model;
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<ChatModel>> GetAllChatsAsync(Guid userId)
        {
            // check
            if (userId == default) throw new ArgumentOutOfRangeException(nameof(userId));

            await using var context = _contextFactory.CreateContext();

            var models = await context.Members
                .Where(mem => mem.UserId == userId)
                .Select(mem => new ChatModel
                {
                    Id = mem.Chat!.Id,
                    Created = mem.Chat.Created,
                    IsPrivate = mem.Chat.IsPrivate,
                    Name = !mem.Chat.IsPrivate ? mem.Chat.Name : mem.Chat.ChatUsers!.FirstOrDefault(x => x.UserId != userId)!.User!.Name!,
                    LastMessage = mem.Chat!.Messages!.OrderByDescending(x => x.Created).Select(mes => new MessageModel
                        {
                            Created = mes.Created,
                            Text = mes.Text,
                            AuthorId = mes.AuthorId,
                            AuthorName = mes.Author!.Name,
                            ChatId = mem.ChatId,
                        })
                        .FirstOrDefault(),
                    UnreadCount = mem.Chat!.Messages!.Count(x => x.Created > mem.LastReadDate)
                })
                .ToArrayAsync();

            return models;
        }

        /// <inheritdoc/>
        public async Task<ChatModel> GetChatAsync(UserModel user, Guid chatId)
        {
            await using var context = _contextFactory.CreateContext();
            
            if (user == null) throw new ArgumentNullException(nameof(user));
            await _memberService.ThrowIfAccessDenied(context, user.Id, chatId);

            var chat = await context.Members
                .Where(mem => mem.UserId == user.Id)
                .Where(mem => mem.ChatId == chatId)
                .Select(mem => new ChatModel
                {
                    Id = mem.Chat!.Id,
                    Created = mem.Chat.Created,
                    IsPrivate = mem.Chat.IsPrivate,
                    Name = !mem.Chat.IsPrivate ? mem.Chat.Name : mem.Chat.ChatUsers!.FirstOrDefault(x => x.UserId != user.Id)!.User!.Name!,
                    LastMessage = mem.Chat!.Messages!.OrderByDescending(x => x.Created).Select(mes => new MessageModel
                        {
                            Created = mes.Created,
                            Text = mes.Text,
                            AuthorId = mes.AuthorId,
                            AuthorName = mes.Author!.Name,
                            ChatId = mem.ChatId,
                        })
                        .FirstOrDefault(),
                    UnreadCount = mem.Chat!.Messages!.Count(x => x.Created > mem.LastReadDate)
                })
                .FirstOrDefaultAsync();

            return chat;
        }

        /// <inheritdoc/>
        public async Task RenameChatAsync(UserModel owner, Guid chatId, string name)
        {
            if (owner == null) throw new ArgumentNullException(nameof(owner));
            if (!name.HasValue()) throw new ArgumentNullException(nameof(name));
            
            _logger.LogDebug($"{nameof(RenameChatAsync)} started by user with id: {owner.Id} for chatId: {chatId}, name: {name}");
            await using var context = _contextFactory.CreateContext();

            // checks
            var chat = await context.Chats
                           .SingleOrDefaultAsync(x => x.Id == chatId)
                       ?? throw new EntityNotFoundException($"Chat with id: {chatId} was not found.");
            
            if (chat.OwnerId != owner.Id) throw new ForbiddenException("Can't update the chat because the current user is not the owner.");
            if (chat.IsPrivate) throw new ChatLogicException("Can't change chat name because private chat doesn't have name.");

            // update
            chat.Name = name;
            await context.SaveChangesAsync();
            
            _logger.LogInformation($"{nameof(RenameChatAsync)} successfully renamed chat Id: {chatId}, name: {name}");
        }

        /// <inheritdoc/>
        public async Task ChangeOwnerAsync(UserModel owner, Guid chatId, Guid newOwnerId)
        {
            if (owner == null) throw new ArgumentNullException(nameof(owner));
            
            _logger.LogDebug($"{nameof(ChangeOwnerAsync)} started by user with id: {owner.Id} for chat id: {chatId}, newOwnerId: {newOwnerId}");
            await using var context = _contextFactory.CreateContext();
            
            // check new owner
            if (!await context.Users.AnyAsync(x => x.Id == newOwnerId))
                throw new EntityNotFoundException($"User with id: {newOwnerId} was not found.");
            
            // check chat
            var chat = await context.Chats
                           .SingleOrDefaultAsync(x => x.Id == chatId)
                       ?? throw new EntityNotFoundException($"Chat with id: {chatId} was not found.");

            if (chat.OwnerId != owner.Id) throw new ForbiddenException("Can't update the chat because the current user is not the owner.");
            if (chat.IsPrivate) throw new ChatLogicException("Can't change private chat's owner.");

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
            
            var chat = await context.Chats
                           .SingleOrDefaultAsync(x => x.Id == chatId)
                       ?? throw new EntityNotFoundException($"Chat with id: {chatId} was not found.");
            
            if (chat.OwnerId != owner.Id) throw new ForbiddenException("Can't remove the chat because the current user is not the owner.");
            if (chat.IsPrivate) throw new ChatLogicException("Can't remove private chat.");

            // remove
            context.Remove(chat);
            await context.SaveChangesAsync();

            _logger.LogInformation($"{nameof(RemovePublicAsync)} successfully remove chat with id: {chatId}");
        }
    }
}