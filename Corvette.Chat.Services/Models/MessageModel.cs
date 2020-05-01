﻿using System;
using Corvette.Chat.Data.Entities;

namespace Corvette.Chat.Services.Models
{
    /// <summary>
    /// A message which a user adds to a chat.
    /// </summary>
    public sealed class MessageModel
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
        /// Create a new <see cref="MessageModel"/>
        /// </summary>
        public MessageModel(MessageEntity entity)
        {
            if (entity.Author == null) throw new ArgumentNullException(nameof(entity.Author));
            
            Id = entity.Id;
            Created = entity.Created;
            Text = entity.Text;
            AuthorId = entity.AuthorId;
            AuthorName = entity.Author.Name;
            ChatId = entity.ChatId;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, " +
                   $"{nameof(Created)}: {Created}, " +
                   $"{nameof(Text)}: {Text}, " +
                   $"{nameof(AuthorId)}: {AuthorId}, " +
                   $"{nameof(AuthorName)}: {AuthorName}, " +
                   $"{nameof(ChatId)}: {ChatId}";
        }
    }
}