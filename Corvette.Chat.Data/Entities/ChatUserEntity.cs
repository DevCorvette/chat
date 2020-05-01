using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corvette.Chat.Data.Entities
{
    /// <summary>
    /// Relation between chat and user.
    /// </summary>
    [Table("ChatUsers")]
    public sealed class ChatUserEntity : BaseEntity
    {
        /// <summary>
        /// User id.
        /// </summary>
        public Guid UserId { get; set; }
        
        /// <summary>
        /// Chat participant.
        /// </summary>
        public UserEntity? User { get; set; }
        
        /// <summary>
        /// Chat id.
        /// </summary>
        public Guid ChatId { get; set; }
        
        /// <summary>
        /// A chat.
        /// </summary>
        public ChatEntity? Chat { get; set; }
        
        /// <summary>
        /// Date when the user last see the chat.
        /// It's date of creating entity by default.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime LastReadDate { get; set; }
    }
}