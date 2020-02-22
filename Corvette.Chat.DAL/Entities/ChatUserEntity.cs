using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corvette.Chat.DAL.Entities
{
    /// <summary>
    /// Relation between chat and user.
    /// </summary>
    [Table("ChatUsers")]
    public class ChatUserEntity : BaseEntity
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
        /// Id of the last message which the user read.
        /// </summary>
        public Guid? LastReadMessageId { get; set; }
        
        /// <summary>
        /// The last message which the user read in the chat.
        /// </summary>
        public MessageEntity? LastReadMessage { get; set; }
    }
}