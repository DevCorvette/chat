using System.ComponentModel.DataAnnotations;

namespace Corvette.Chat.WebService.Models.Users
{
    /// <summary>
    /// Request model for creating a new chat user.
    /// </summary>
    public class CreateUserRequest
    {
        /// <summary>
        /// User name.
        /// Needs to be unique.
        /// </summary>
        [Required]
        [MinLength(1)]
        [MaxLength(200)]
        public string Name { get; set; } = null!;

        /// <summary>
        /// User login.
        /// Needs to be unique.
        /// </summary>
        [Required]
        [MinLength(1)]
        [MaxLength(200)]
        public string Login { get; set; } = null!;
        
        /// <summary>
        /// User secret key.
        /// It isn't a password.
        /// Use it to bind a user in your database to a user in the chat database.
        /// </summary>
        [Required]
        [MinLength(1)]
        public string Key { get; set; } = null!;
    }
}