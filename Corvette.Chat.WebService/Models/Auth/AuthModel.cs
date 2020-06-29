using System.ComponentModel.DataAnnotations;

namespace Corvette.Chat.WebService.Models.Auth
{
    /// <summary>
    /// Model for authorization
    /// </summary>
    public class AuthModel
    {
        /// <summary>
        /// User's login.
        /// </summary>
        [Required]
        [MinLength(1)]
        public string Login { get; set; }
        
        /// <summary>
        /// User's secret key.
        /// </summary>
        [Required]
        [MinLength(1)]
        public string Key { get; set; }
    }
}