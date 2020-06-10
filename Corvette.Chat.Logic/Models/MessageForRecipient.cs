using System;
using Corvette.Chat.Data.Entities;

namespace Corvette.Chat.Logic.Models
{
    public class MessageForRecipient : MessageModel
    {
        public Guid Recipient { get; }
        
        public int UnreadCount { get; }

        public MessageForRecipient(MessageEntity entity, int unreadCount, Guid recipient) : base(entity)
        {
            Recipient = recipient;
            UnreadCount = unreadCount;
        }

        public MessageForRecipient(MessageEntity entity, string authorName, int unreadCount, Guid recipient) : base(entity, authorName)
        {
            Recipient = recipient;
            UnreadCount = unreadCount;
        }
    }
}