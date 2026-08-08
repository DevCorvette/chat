using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corvette.Chat.Data.Entities
{
    /// <summary>
    /// Chat user.
    /// </summary>
    [Table("users")]
    public class UserEntity : BaseEntity
    {
        /// <summary>
        /// User name.
        /// It's unique.
        /// The maximum name length is 200 chars. 
        /// </summary>
        [Required]
        [StringLength(200)]
        [Column("name")]
        public string Name { get; set; } = null!;
        
        /// <summary>
        /// User login.
        /// It's unique and used for authorization.
        /// The maximum name length is 200 chars. 
        /// </summary>
        [Required]
        [StringLength(200)]
        [Column("login")]
        public string Login { get; set; } = null!;
        
        /// <summary>
        /// A secret key for authorization.
        /// </summary>
        [Required]
        [Column("secret_key")]
        public string SecretKey { get; set; } = null!;
        
        /// <summary>
        /// Messages which the user sent to chats.
        /// </summary>
        public virtual ICollection<MessageEntity>? Messages { get; set; }
        
        /// <summary>
        /// Collection from which we can get user chats.
        /// </summary>
        public virtual ICollection<MemberEntity>? ChatUsers { get; set; }
        
        /// <summary>
        /// User owned chats.
        /// </summary>
        public virtual ICollection<ChatEntity>? OwnChats { get; set; }
    }
}  