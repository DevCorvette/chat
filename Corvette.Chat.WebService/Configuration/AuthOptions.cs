using System.Collections.Generic;
using System.Text;
using Curiosity.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Corvette.Chat.WebService.Configuration
{
    /// <summary>
    /// Options for JWT generation
    /// </summary>
    public class AuthOptions : IValidatableOptions, ILoggableOptions
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

        public IReadOnlyCollection<ConfigurationValidationError> Validate(string? prefix = null)
        {
            var errors = new ConfigurationValidationErrorCollection(prefix);
            
            errors.AddErrorIf(string.IsNullOrWhiteSpace(Issuer), nameof(Issuer), "Can't be empty or null");
            errors.AddErrorIf(string.IsNullOrWhiteSpace(Audience), nameof(Audience), "Can't be empty or null");
            errors.AddErrorIf(string.IsNullOrWhiteSpace(Key), nameof(Key), "Can't be empty or null");
            errors.AddErrorIf(LifeDays < 1, nameof(LifeDays), "Can't be less than 1");
            
            return errors;
        }
    }
}