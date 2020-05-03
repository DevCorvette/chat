using System;
using Corvette.Chat.Data.Entities;

namespace Corvette.Chat.Services.Models
{
    public class MessageForRecipient : MessageModel
    {
        public Guid Recipient { get; }

        public MessageForRecipient(MessageEntity entity, int unreadCount, Guid recipient) : base(entity, unreadCount)
        {
            Recipient = recipient;
        }

        public MessageForRecipient(MessageEntity entity, string authorName, int unreadCount, Guid recipient) : base(entity, authorName, unreadCount)
        {
            Recipient = recipient;
        }
    }
}