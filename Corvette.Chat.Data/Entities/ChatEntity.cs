using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corvette.Chat.Data.Entities
{
    /// <summary>
    /// A place for users conversation.
    /// </summary>
    [Table("chats")]
    public class ChatEntity : BaseEntity
    {
        /// <summary>
        /// When it's true then the chat can contain only two users.
        /// </summary>
        [Column("is_private")]
        public bool IsPrivate { get; set; }
        
        /// <summary>
        /// Chat name.
        /// It's not unique.
        /// It's null then chat is private.
        /// The maximum name length is 200 chars. 
        /// </summary>
        [Column("name")]
        [StringLength(200)]
        public string? Name { get; set; }
        
        /// <summary>
        /// Id of a user who can edit the chat.
        /// </summary>
        [Column("owner_id")]
        public Guid OwnerId { get; set; }
        
        /// <summary>
        /// A user who can edit the chat.
        /// </summary>
        public virtual UserEntity? Owner { get; set; }
        
        /// <summary>
        /// Messages which users wrote into the chat.
        /// </summary>
        public virtual ICollection<MessageEntity>? Messages { get; set; }
        
        /// <summary>
        /// Collection from which we can get chat users.
        /// </summary>
        public virtual ICollection<MemberEntity>? ChatUsers { get; set; }
    }
}