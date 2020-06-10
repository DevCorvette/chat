using System;
using Corvette.Chat.Data.Entities;

namespace Corvette.Chat.Logic.Models
{
    /// <summary>
    /// A message which a user adds to a chat.
    /// </summary>
    public class MessageModel
    {
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
        /// Create a new <see cref="MessageModel"/>.
        /// Needs to include an author inside the entity.
        /// </summary>
        public MessageModel(MessageEntity entity)
        {
            if (entity.Author == null) throw new ArgumentNullException(nameof(entity.Author));
            
            Created = entity.Created;
            Text = entity.Text;
            AuthorId = entity.AuthorId;
            AuthorName = entity.Author.Name;
            ChatId = entity.ChatId;
        }
        
        /// <summary>
        /// Create a new <see cref="MessageModel"/>
        /// </summary>
        public MessageModel(MessageEntity entity, string authorName)
        {
            Created = entity.Created;
            Text = entity.Text;
            AuthorId = entity.AuthorId;
            AuthorName = authorName;
            ChatId = entity.ChatId;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{nameof(Created)}: {Created}, " +
                   $"{nameof(Text)}: {Text}, " +
                   $"{nameof(AuthorId)}: {AuthorId}, " +
                   $"{nameof(AuthorName)}: {AuthorName}, " +
                   $"{nameof(ChatId)}: {ChatId}";
        }
    }
}