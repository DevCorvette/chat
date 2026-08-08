using System.Collections.Generic;
using Curiosity.Configuration;
using Curiosity.DAL;
using Curiosity.Hosting.Web;

namespace Corvette.Chat.WebService.Configuration
{
    public class WebServiceConfiguration : CuriosityWebAppConfiguration
    {
        public WebServiceConfiguration()
        {
            AppName ??= "Corvette chat web service";
        }
        
        /// <summary>
        /// List of URL from which requests are going to make.
        /// Used for CORS policy
        /// </summary>
        public List<string> AllowedUrls { get; set; } = new List<string>();

        /// <summary>
        /// Options for JWT generation
        /// </summary>
        public AuthOptions AuthOptions { get; set; } = new AuthOptions();
        
        /// <summary>
        /// Data base options
        /// </summary>
        public DbOptions DbOptions { get; set; } = new DbOptions();

        public override IReadOnlyCollection<ConfigurationValidationError> Validate(string? prefix = null)
        {
            var errors = new ConfigurationValidationErrorCollection(prefix);
            
            errors.AddErrors(base.Validate(prefix));
            errors.AddErrors(AuthOptions.Validate(nameof(AuthOptions)));
            errors.AddErrors(DbOptions.Validate(nameof(DbOptions)));
            
            for (int i = 0; i < AllowedUrls.Count; i++)
            {
                errors.AddErrorIf(string.IsNullOrWhiteSpace(AllowedUrls[i]), $"{nameof(AllowedUrls)}[{i}]", "Can't be empty or null");
            }

            return errors;
        }
    }
}