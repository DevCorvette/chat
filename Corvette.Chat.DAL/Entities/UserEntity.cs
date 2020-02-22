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
        /// The maximum name length is 200 chars. 
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Name { get; set; }
    }
}  