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
        public Guid Id { get; }
        
        /// <summary>
        /// Date when chat was created.
        /// </summary>
        public DateTime Created { get; }
        
        /// <summary>
        /// When it's true then the chat can contain only two users.
        /// </summary>
        public bool IsPrivate { get; }
        
        /// <summary>
        /// Chat name.
        /// </summary>
        public string? Name { get; }
        
        /// <summary>
        /// The last message in the chat.
        /// </summary>
        public MessageModel? LastMessage { get; }
        
        /// <summary>
        /// The count of unread messages for current user.
        /// </summary>
        public int UnreadCount { get; }
        
        /// <summary>
        /// Create a new <see cref="ChatModel"/>
        /// </summary>
        public ChatModel(ChatEntity chat, string chatName, MessageModel? lastMessage, int unreadCount)
        {
            if (string.IsNullOrWhiteSpace(chatName)) throw new ArgumentOutOfRangeException(nameof(chatName));
            
            Id = chat.Id;
            Created = chat.Created;
            IsPrivate = chat.IsPrivate;
            Name = chatName;
            LastMessage = lastMessage;
            UnreadCount = unreadCount;
        }

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