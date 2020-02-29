using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corvette.Chat.DAL.Entities
{
    /// <summary>
    /// Chat user.
    /// </summary>
    [Table("Users")]
    public class UserEntity : BaseEntity
    {
        /// <summary>
        /// User name.
        /// It's unique.
        /// The maximum name length is 200 chars. 
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = null!;
        
        /// <summary>
        /// Messages which the user sent to chats.
        /// </summary>
        public ICollection<MessageEntity>? Messages { get; set; }
        
        /// <summary>
        /// Collection from which we can get user chats.
        /// </summary>
        public ICollection<ChatUserEntity>? ChatUsers { get; set; }
        
    }
}  