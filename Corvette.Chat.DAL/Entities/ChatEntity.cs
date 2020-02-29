using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corvette.Chat.DAL.Entities
{
    /// <summary>
    /// A place for users conversation.
    /// </summary>
    [Table("Chats")]
    public class ChatEntity : BaseEntity
    {
        /// <summary>
        /// When it's true then the chat can contain only two users.
        /// </summary>
        public bool IsPrivate { get; set; }
        
        /// <summary>
        /// Chat name.
        /// It's not unique.
        /// It's null then chat is private.
        /// The maximum name length is 200 chars. 
        /// </summary>
        [StringLength(200)]
        public string? Name { get; set; }
        
        /// <summary>
        /// Messages which users wrote into the chat.
        /// </summary>
        public ICollection<MessageEntity>? Messages { get; set; }
        
        /// <summary>
        /// Collection from which we can get chat users.
        /// </summary>
        public ICollection<ChatUserEntity>? ChatUsers { get; set; }
    }
}