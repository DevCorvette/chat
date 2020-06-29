using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Corvette.Chat.WebService.Settings
{
    /// <summary>
    /// Settings for JWT authorization
    /// </summary>
    public class AuthSettings
    { 
        /// <summary>
        /// Issuer of token.
        /// </summary>
        public string Issuer { get; set; } = null!;
        
        /// <summary>
        /// Audience of token.
        /// </summary>
        public string Audience { get; set; } = null!;
        
        /// <summary>
        /// Key for generate symmetric security key.
        /// </summary>
        public string Key { get; set; } = null!;
        
        /// <summary>
        /// Token expiration time.
        /// </summary>
        public int LifeDays { get; set; }

        /// <summary>
        /// Gets SymmetricSecurityKey by current string key.
        /// </summary>
        public SymmetricSecurityKey SymmetricSecurityKey => new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
    }
}