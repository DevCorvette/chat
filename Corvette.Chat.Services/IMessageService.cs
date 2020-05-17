using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Corvette.Chat.Services.Models;

namespace Corvette.Chat.Services
{
    public interface IMessageService
    {
        /// <summary>
        /// Adds the text like a message to the chat.
        /// Returns list of messages for each recipient with an unread count.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        Task<IReadOnlyList<MessageForRecipient>> AddMessageAsync(UserModel author, Guid chatId, string text);

        /// <summary>
        /// Returns half read and half unread messages of the chat.
        /// Use this method to entering in the chat.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        Task<IReadOnlyList<MessageModel>> GetLastWithUnread(UserModel user, Guid chatId, int take);
        
        /// <summary>
        /// Returns chat messages with skip and take params.
        /// Use this method for scroll up or down.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        Task<IReadOnlyList<MessageModel>> GetMessagesAsync(UserModel user, Guid chatId, DateTime skip, int take, bool isSkipTop);
    }
}