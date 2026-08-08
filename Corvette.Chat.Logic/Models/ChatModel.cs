using System;
using Corvette.Chat.Data.Entities;

namespace Corvette.Chat.Logic.Models
{
    /// <summary>
    /// A place for users conversation.
    /// </summary>
    public sealed class ChatModel
    {
        /// <summary>
        /// Chat id.
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Date when chat was created.
        /// </summary>
        public DateTime Created { get; set; }
        
        /// <summary>
        /// When it's true then the chat can contain only two users.
        /// </summary>
        public bool IsPrivate { get; set; }
        
        /// <summary>
        /// Chat name.
        /// </summary>
        public string? Name { get; set; }
        
        /// <summary>
        /// The last message in the chat.
        /// </summary>
        public MessageModel? LastMessage { get; set; }
        
        /// <summary>
        /// The count of unread messages for current user.
        /// </summary>
        public int UnreadCount { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, " +
                   $"{nameof(Created)}: {Created}, " +
                   $"{nameof(IsPrivate)}: {IsPrivate}, " +
                   $"{nameof(Name)}: {Name}, " +
                   $"{nameof(LastMessage)}: {LastMessage}, " +
                   $"{nameof(UnreadCount)}: {UnreadCount}";
        }
    }
}