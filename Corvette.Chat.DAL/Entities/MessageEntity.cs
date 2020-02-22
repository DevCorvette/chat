using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corvette.Chat.DAL.Entities
{
    /// <summary>
    /// A message which a user adds to a chat.
    /// </summary>
    [Table("Messages")]
    public class MessageEntity : BaseEntity
    {
        /// <summary>
        /// Message text.
        /// The maximum text length is 3000 chars. 
        /// </summary>
        [Required]
        [StringLength(3000)]
        public string Text { get; set; }
        
        /// <summary>
        /// Id of a user who write the message.
        /// </summary>
        public Guid AuthorId { get; set; }
        
        /// <summary>
        /// An user who write the message.
        /// </summary>
        public UserEntity? Author { get; set; }
        
        /// <summary>
        /// Id of a chat into which a user wrote the message.
        /// </summary>
        public Guid ChatId { get; set; }
        
        /// <summary>
        /// A chat into which a user wrote the message.
        /// </summary>
        public ChatEntity? Chat { get; set; }
    }
}