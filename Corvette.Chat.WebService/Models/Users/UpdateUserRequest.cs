using System;
using System.ComponentModel.DataAnnotations;

namespace Corvette.Chat.WebService.Models.Users
{
    /// <summary>
    /// Request model for updating a new chat user.
    /// </summary>
    public class UpdateUserRequest
    {
        /// <summary>
        /// User's id.
        /// </summary>
        public Guid UserId { get; set; }
        
        /// <summary>
        /// User name.
        /// Needs to be unique.
        /// Doesn't update if it's null.
        /// </summary>
        [MinLength(1)]
        [MaxLength(200)]
        public string? Name { get; set; }

        /// <summary>
        /// User login.
        /// Needs to be unique.
        /// Doesn't update if it's null.
        /// </summary>
        [MinLength(1)]
        [MaxLength(200)]
        public string? Login { get; set; }
        
        /// <summary>
        /// User secret key.
        /// It isn't a password.
        /// Use it to bind a user in your database to a user in the chat database.
        /// </summary>
        [MinLength(1)]
        public string? Key { get; set; }
    }
}