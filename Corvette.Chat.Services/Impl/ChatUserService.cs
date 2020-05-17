using System;
using System.Linq;
using System.Threading.Tasks;
using Corvette.Chat.Data;
using Corvette.Chat.Services.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Corvette.Chat.Services.Impl
{
    public class ChatUserService : IChatUserService
    {
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
    }
}