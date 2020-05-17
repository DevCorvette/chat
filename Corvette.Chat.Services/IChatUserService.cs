using System;
using System.Threading.Tasks;
using Corvette.Chat.Data;
using Corvette.Chat.Services.Exceptions;

namespace Corvette.Chat.Services
{
    public interface IChatUserService
    {
        /// <summary>
        /// Throws Forbidden exception when can't find the user in the chat.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ForbiddenException"></exception>
        Task ThrowIfAccessDenied(ChatDataContext context, Guid userId, Guid chatId);
    }
}