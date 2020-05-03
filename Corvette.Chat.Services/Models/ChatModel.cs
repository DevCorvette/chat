using System;
using Corvette.Chat.Data.Entities;

namespace Corvette.Chat.Services.Models
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
        /// Create a new <see cref="ChatModel"/>
        /// </summary>
        public ChatModel(ChatEntity chat, string chatName, MessageModel? lastMessage)
        {
            if (string.IsNullOrWhiteSpace(chatName)) throw new ArgumentOutOfRangeException(nameof(chatName));
            
            Id = chat.Id;
            Created = chat.Created;
            IsPrivate = chat.IsPrivate;
            Name = chatName;
            LastMessage = lastMessage;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, " +
                   $"{nameof(Created)}: {Created}, " +
                   $"{nameof(IsPrivate)}: {IsPrivate}, " +
                   $"{nameof(Name)}: {Name}, " +
                   $"{nameof(LastMessage)}: {LastMessage}";
        }
    }
}