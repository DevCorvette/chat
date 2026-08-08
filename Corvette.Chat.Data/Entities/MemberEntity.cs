using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corvette.Chat.Data.Entities
{
    /// <summary>
    /// Relation between chat and user.
    /// </summary>
    [Table("members")]
    public class MemberEntity : BaseEntity
    {
        /// <summary>
        /// User id.
        /// </summary>
        [Column("user_id")]
        public Guid UserId { get; set; }
        
        /// <summary>
        /// Chat participant.
        /// </summary>
        public virtual UserEntity? User { get; set; }
        
        /// <summary>
        /// Chat id.
        /// </summary>
        [Column("chat_id")]
        public Guid ChatId { get; set; }
        
        /// <summary>
        /// A chat.
        /// </summary>
        public virtual ChatEntity? Chat { get; set; }
        
        /// <summary>
        /// Date when the user last see the chat.
        /// It's date of creating entity by default.
        /// </summary>
        [Column("last_read_date")]
        public DateTime LastReadDate { get; set; }
    }
}