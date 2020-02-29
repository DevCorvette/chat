using System;
using Corvette.Chat.Data.Entities;

namespace Corvette.Chat.Services.DTO
{
    /// <summary>
    /// A message which a user adds to a chat.
    /// </summary>
    public sealed class MessageDto
    {
        /// <summary>
        /// Message id.
        /// </summary>
        public Guid Id { get; }
        
        /// <summary>
        /// Date when message was created.
        /// </summary>
        public DateTime Created { get; }
        
        /// <summary>
        /// Message text.
        /// </summary>
        public string Text { get; }
        
        /// <summary>
        /// Id of a user who write the message.
        /// </summary>
        public Guid AuthorId { get; }
        
        /// <summary>
        /// Name of user who write the message.
        /// </summary>
        public string AuthorName { get; }
        
        /// <summary>
        /// Id of a chat into which a user wrote the message.
        /// </summary>
        public Guid ChatId { get; }
        
        /// <summary>
        /// Name of chat into which a user wrote the message.
        /// </summary>
        public string ChatName { get; }

        /// <summary>
        /// Create a new <see cref="MessageDto"/>
        /// </summary>
        public MessageDto(MessageEntity entity, string authorName, string chatName)
        {
            Id = entity.Id;
            Created = entity.Created;
            Text = entity.Text;
            AuthorId = entity.AuthorId;
            AuthorName = authorName;
            ChatId = entity.ChatId;
            ChatName = chatName;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, " +
                   $"{nameof(Created)}: {Created}, " +
                   $"{nameof(Text)}: {Text}, " +
                   $"{nameof(AuthorId)}: {AuthorId}, " +
                   $"{nameof(AuthorName)}: {AuthorName}, " +
                   $"{nameof(ChatId)}: {ChatId}, " +
                   $"{nameof(ChatName)}: {ChatName}";
        }
    }
}