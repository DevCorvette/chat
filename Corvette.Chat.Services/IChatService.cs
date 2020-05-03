using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Corvette.Chat.Services.Exceptions;
using Corvette.Chat.Services.Models;

namespace Corvette.Chat.Services
{
    /// <summary>
    /// Chat service.
    /// </summary>
    public interface IChatService
    {
        /// <summary>
        /// Creates new public chat.
        /// </summary>
        /// <param name="name">Chat name</param>
        /// <param name="creator">Chat's creator</param>
        /// <exception cref="ArgumentNullException"></exception>
        Task<ChatModel> CreatePublicChatAsync(UserModel creator, string name);

        /// <summary>
        /// Creates new private chat with interlocutor.
        /// </summary>
        /// <param name="creator">Chat's creator</param>
        /// <param name="interlocutorId">Interlocutor id</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="EntityNotFoundException"></exception>
        Task<ChatModel> CreatePrivateChatAsync(UserModel creator, Guid interlocutorId);
        
        /// <summary>
        /// Returns all chats where the current user is a member.
        /// </summary>
        /// <param name="userId">User id</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        Task<IReadOnlyList<ChatModel>> GetAllChatsAsync(Guid userId);

        /// <summary>
        /// Returns chat by id for the user.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="EntityNotFoundException"></exception>
        Task<ChatModel> GetChatAsync(UserModel user, Guid chatId);

        /// <summary>
        /// Renames a public chat.
        /// </summary>
        /// <param name="owner">Chat's owner</param>
        /// <param name="chatId">Chat id</param>
        /// <param name="name">New chat name</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ForbiddenException">When action user isn't a chat owner.</exception>
        /// <exception cref="EntityNotFoundException"></exception>
        /// <exception cref="ChatServiceException"></exception>
        Task RenameChatAsync(UserModel owner, Guid chatId, string name);

        /// <summary>
        /// Updates chat's owner or name.
        /// Updates only those property that isn't null.
        /// </summary>
        /// <param name="owner">Chat's owner</param>
        /// <param name="chatId">Chat id</param>
        /// <param name="newOwnerId"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ForbiddenException">When action user isn't a chat owner.</exception>
        /// <exception cref="EntityNotFoundException"></exception>
        Task ChangeOwnerAsync(UserModel owner, Guid chatId, Guid newOwnerId);

        /// <summary>
        /// Removes a public chat.
        /// </summary>
        /// <param name="owner">Chat's owner</param>
        /// <param name="chatId">Chat id</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ForbiddenException">When action user isn't a chat owner.</exception>
        /// <exception cref="ChatServiceException">When action user isn't a chat owner.</exception>
        Task RemovePublicAsync(UserModel owner, Guid chatId);
    }
}