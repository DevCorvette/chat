using System;
using Corvette.Chat.Data.Entities;

namespace Corvette.Chat.Services.DTO
{
    /// <summary>
    /// A place for users conversation.
    /// </summary>
    public sealed class ChatDto
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
        /// Create a new <see cref="ChatDto"/>
        /// </summary>
        public ChatDto(ChatEntity entity)
        {
            Id = entity.Id;
            Created = entity.Created;
            IsPrivate = entity.IsPrivate;
            Name = entity.Name;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, " +
                   $"{nameof(Created)}: {Created}, " +
                   $"{nameof(IsPrivate)}: {IsPrivate}, " +
                   $"{nameof(Name)}: {Name}";
        }
    }
}