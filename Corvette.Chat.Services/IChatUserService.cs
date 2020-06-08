using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Corvette.Chat.Data;
using Corvette.Chat.Services.Exceptions;
using Corvette.Chat.Services.Models;

namespace Corvette.Chat.Services
{
    public interface IChatUserService
    {
        /// <summary>
        /// Throws Forbidden exception when can't find the user in the chat.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ForbiddenException"></exception>
        Task ThrowIfAccessDenied(ChatDataContext context, Guid userId, Guid chatId);

        /// <summary>
        /// Returns list of members.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        Task<IReadOnlyList<UserModel>> GetMembersAsync(UserModel user, Guid chatId);

        /// <summary>
        /// Adds members to the public chat.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ForbiddenException"></exception>
        /// <exception cref="EntityNotFoundException"></exception>
        /// <exception cref="ChatServiceException"></exception>
        Task AddMembersAsync(UserModel owner, Guid chatId, IReadOnlyList<Guid> newMemberIds);
        
        /// <summary>
        /// Removes members from the public chat.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ForbiddenException"></exception>
        /// <exception cref="EntityNotFoundException"></exception>
        /// <exception cref="ChatServiceException"></exception>
        Task RemoveMembersAsync(UserModel owner, Guid chatId, IReadOnlyList<Guid> memberIds);

        /// <summary>
        /// Removes a user from the public chat.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="EntityNotFoundException"></exception>
        Task LeaveChatAsync(UserModel user, Guid chatId);
    }
}