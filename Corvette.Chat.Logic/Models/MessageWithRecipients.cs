using System;
using System.Collections.Generic;

namespace Corvette.Chat.Logic.Models
{
    public class MessageWithRecipients 
    {
        /// <summary>
        /// New chat message.
        /// </summary>
        public MessageModel Message { get; }
        
        /// <summary>
        /// List with an id of users who should receive the message.
        /// </summary>
        public IReadOnlyList<MessageRecipient> Recipients { get; }

        public MessageWithRecipients(MessageModel message, IReadOnlyList<MessageRecipient> recipients)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Recipients = recipients ?? throw new ArgumentNullException(nameof(recipients));
        }
    }

    public class MessageRecipient
    {
        /// <summary>
        /// Id of a user who should receive tha message.
        /// </summary>
        public Guid Id { get; }
        
        /// <summary>
        /// Count of unread message for the user in the current chat.
        /// </summary>
        public int UnreadCount { get; }

        public MessageRecipient(Guid id, int unreadCount)
        {
            if (id == Guid.Empty) throw new ArgumentOutOfRangeException(nameof(id));
            if (unreadCount < 0) throw new ArgumentOutOfRangeException(nameof(unreadCount));
            
            Id = id;
            UnreadCount = unreadCount;
        }
    }
}