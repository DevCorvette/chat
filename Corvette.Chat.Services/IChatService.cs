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
        /// Creates new public or private chat.
        /// </summary>
        /// <param name="name">Chat name</param>
        /// <param name="creator">Chat's creator</param>
        /// <param name="memberIds">Members</param>
        /// <param name="isPrivate">Is chat private</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="EntityNotFoundException">When chat member not found by id.</exception>
        /// <exception cref="ChatServiceException">When memberIds contain more than 1 id for private chat.</exception>
        Task<ChatModel> CreateChatAsync(UserModel creator, string? name, IReadOnlyList<Guid> memberIds, bool isPrivate);
        
        /// <summary>
        /// Returns all chats where the current user is a member.
        /// </summary>
        /// <param name="userId">User id</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        Task<IReadOnlyList<ChatModel>> GetAllChatsAsync(Guid userId);

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
        Task RemoveAsync(UserModel owner, Guid chatId);
    }
}