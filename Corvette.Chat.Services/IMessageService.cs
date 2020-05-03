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
        /// Returns messages by chat.
        /// </summary>
        /// <param name="chatId">Id of chat</param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        Task<IReadOnlyList<MessageModel>> GetMessagesAsync(Guid chatId, DateTime? skip, int take);
    }
}