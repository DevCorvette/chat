using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Corvette.Chat.Logic;
using Corvette.Chat.Logic.Models;
using Corvette.Chat.WebService.Helpers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Hub = Microsoft.AspNetCore.SignalR.Hub;

namespace Corvette.Chat.WebService.SignalR
{
    public class ChatHub : Hub
    {
        private readonly AuthHelper _authHelper;
        
        private readonly ILogger<ChatHub> _logger;

        private readonly IChatService _chatService;

        private readonly IMessageService _messageService;

        private readonly IMemberService _memberService;

        public ChatHub(
            AuthHelper authHelper, 
            ILogger<ChatHub> logger, 
            IChatService chatService, 
            IMessageService messageService, 
            IMemberService memberService)
        {
            _authHelper = authHelper ?? throw new ArgumentNullException(nameof(authHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _chatService = chatService ?? throw new ArgumentNullException(nameof(chatService));
            _messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
            _memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                var connectionId = Context.ConnectionId;
                _logger.LogDebug($"Trying to connect to the chat hub. ConnectionId: {connectionId}");
                
                var httpContext = Context.GetHttpContext() ?? throw new InvalidOperationException("Need HTTP context for connection");
                var user = await _authHelper.GetUserAsync(httpContext);

                Context.Items[connectionId] = user;
            
                await base.OnConnectedAsync();
                _logger.LogInformation($"User with Id: {user.Id}, connectionId: {connectionId} connected to the ChatHub. Total connections are {Context.Items.Count}");
            }
            catch (Exception e)
            {
                throw ExceptionHandler(e);
            }
        }
        
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            try
            {
                var connectionId = Context.ConnectionId;
                Context.Items.Remove(connectionId);
            
                _logger.LogInformation($"User with connectionId: {connectionId} disconnected from the ChatHub. Total connections are {Context.Items.Count}");
                await base.OnDisconnectedAsync(exception);
            }
            catch (Exception e)
            {
                throw ExceptionHandler(e);
            }
        }
        
        /// <summary>
        /// Returns all chats where the current user is a member.
        /// </summary>
        [HubMethodName("GetAllChats")]
        public async Task<IReadOnlyList<ChatModel>> GetAllChats()
        {
            try
            {
                var user = GetConnectedUser();
                return await _chatService.GetAllChatsAsync(user.Id);
            }
            catch (Exception e)
            {
                throw ExceptionHandler(e);
            }
        }
        
        /// <summary>
        /// Returns chat by id for the user.
        /// </summary>
        [HubMethodName("GetChat")]
        public async Task<ChatModel> GetChat(Guid chatId)
        {
            try
            {
                var user = GetConnectedUser();
                return await _chatService.GetChatAsync(user, chatId);
            }
            catch (Exception e)
            {
                throw ExceptionHandler(e);
            }
        }

        /// <summary>
        /// Creates new public chat.
        /// </summary>
        [HubMethodName("CreatePublicChat")]
        public async Task<ChatModel> CreatePublicChat(string chatName)
        {
            try
            {
                var creator = GetConnectedUser();
                return await _chatService.CreatePublicChatAsync(creator, chatName);
            }
            catch (Exception e)
            {
                throw ExceptionHandler(e);
            }
        }
        
        /// <summary>
        /// Creates new private chat with interlocutor.
        /// </summary>
        [HubMethodName("CreatePrivateChat")]
        public async Task<ChatModel> CreatePrivateChat(Guid interlocutorId)
        {
            try
            {
                var creator = GetConnectedUser();
                return await _chatService.CreatePrivateChatAsync(creator, interlocutorId);
            }
            catch (Exception e)
            {
                throw ExceptionHandler(e);
            }
        }

        /// <summary>
        /// Renames a public chat.
        /// </summary>
        [HubMethodName("RenameChat")]
        public async Task RenameChat(Guid chatId, string chatName)
        {
            try
            {
                var owner = GetConnectedUser();
                await _chatService.RenameChatAsync(owner, chatId, chatName);
            }
            catch (Exception e)
            {
                throw ExceptionHandler(e);
            }
        }

        /// <summary>
        /// Updates owner of a public chat.
        /// </summary>
        [HubMethodName("ChangeChatOwner")]
        public async Task ChangeChatOwner(Guid chatId, Guid newOwnerId)
        {
            try
            {
                var owner = GetConnectedUser();
                await _chatService.ChangeOwnerAsync(owner, chatId, newOwnerId);
            }
            catch (Exception e)
            {
                throw ExceptionHandler(e);
            }
        }

        /// <summary>
        /// Removes a public chat.
        /// </summary>
        [HubMethodName("RemovePublicChat")]
        public async Task RemovePublicChat(Guid chatId)
        {
            try
            {
                var owner = GetConnectedUser();
                await _chatService.RemovePublicAsync(owner, chatId);
            }
            catch (Exception e)
            {
                throw ExceptionHandler(e);
            }
        }

        /// <summary>
        /// Returns list of members.
        /// </summary>
        [HubMethodName("GetMembers")]
        public async Task<IReadOnlyList<UserModel>> GetMembers(Guid chatId)
        {
            try
            {
                var member = GetConnectedUser();
                return await _memberService.GetMembersAsync(member, chatId);
            }
            catch (Exception e)
            {
                throw ExceptionHandler(e);
            }
        }

        /// <summary>
        /// Adds members to the public chat.
        /// </summary>
        [HubMethodName("AddMembers")]
        public async Task AddMembers(Guid chatId, IReadOnlyList<Guid> newMemberIds)
        {
            try
            {
                var owner = GetConnectedUser();
                await _memberService.AddMembersAsync(owner, chatId, newMemberIds);
            }
            catch (Exception e)
            {
                throw ExceptionHandler(e);
            }
        }

        /// <summary>
        /// Removes members from the public chat.
        /// </summary>
        [HubMethodName("RemoveMembers")]
        public async Task RemoveMembers(Guid chatId, IReadOnlyList<Guid> memberIds)
        {
            try
            {
                var owner = GetConnectedUser();
                await _memberService.RemoveMembersAsync(owner, chatId, memberIds);
            }
            catch (Exception e)
            {
                throw ExceptionHandler(e);
            }
        }

        /// <summary>
        /// Removes a user from the public chat.
        /// </summary>
        [HubMethodName("LeaveChat")]
        public async Task LeaveChat(Guid chatId)
        {
            try
            {
                var user = GetConnectedUser();
                await _memberService.LeaveChatAsync(user, chatId);
            }
            catch (Exception e)
            {
                throw ExceptionHandler(e);
            }
        }

        /// <summary>
        /// Adds the text like a message to the chat.
        /// Returns list of messages for each recipient with an unread count.
        /// </summary>
        [HubMethodName("AddMessage")]
        public async Task<MessageModel> AddMessage( Guid chatId, string text)
        {
            try
            {
                // save message
                var author = GetConnectedUser();
                var resp = await _messageService.AddMessageAsync(author, chatId, text);

                // send to all connected recipients
                foreach (var recipient in resp.Recipients)
                {
                    var connections = GetUserConnections(recipient.Id);
                    foreach (var connectionId in connections)
                    {
                        await Clients.Clients(connectionId).SendAsync("ReceiveMessage", resp.Message, recipient.UnreadCount);
                    }
                }

                return resp.Message;
            }
            catch (Exception e)
            {
                throw ExceptionHandler(e);
            }
        }

        /// <summary>
        /// Returns half read and half unread messages of the chat.
        /// Use this method to entering in the chat.
        /// </summary>
        [HubMethodName("GetLastWithUnread")]
        public async Task<IReadOnlyList<MessageModel>> GetLastWithUnread(Guid chatId, int take)
        {
            try
            {
                var member = GetConnectedUser();
                return await _messageService.GetLastWithUnreadAsync(member, chatId, take);
            }
            catch (Exception e)
            {
                throw ExceptionHandler(e);
            }
        }

        /// <summary>
        /// Returns chat messages with skip and take params.
        /// Use this method for scroll up or down.
        /// </summary>
        [HubMethodName("GetMessages")]
        public async Task<IReadOnlyList<MessageModel>> GetMessages(Guid chatId, DateTime skip, int take, bool isSkipTop)
        {
            try
            {
                var member = GetConnectedUser();
                return await _messageService.GetMessagesAsync(member, chatId, skip, take, isSkipTop);
            }
            catch (Exception e)
            {
                throw ExceptionHandler(e);
            }
        }

        /// <summary>
        /// Sets the last read date for a member of a chat.
        /// </summary>
        [HubMethodName("SetLastReadDate")]
        public async Task SetLastReadDate(Guid chatId, DateTime date)
        {
            try
            {
                var user = GetConnectedUser();
                await _memberService.SetLastReadDateAsync(user, chatId, date);
            }
            catch (Exception e)
            {
                throw ExceptionHandler(e);
            }
        }

        private UserModel GetConnectedUser()
        {
            Context.Items.TryGetValue(Context.ConnectionId, out var user);
            return user as UserModel ?? throw new InvalidOperationException("User hasn't connected");
        }
        
        /// <summary>
        /// A user can has many connections.
        /// </summary>
        private IReadOnlyList<string> GetUserConnections(Guid userId)
        {
            return Context.Items
                .Where(x => (x.Value as UserModel)?.Id == userId)
                .Select(x => x.Key)
                .OfType<string>()
                .ToList();
        }

        private HubException ExceptionHandler(Exception e)
        {
            _logger.LogError(e, $"Exception occurred while executing chat hub: \"{e.Message}\"");
            return new HubException(e.Message);
        }
        
    }
}