namespace Corvette.Chat.WebService.Settings
{
    public class AppSettings
    {
        /// <summary>
        /// List of URL from which requests are going to make.
        /// </summary>
        public string[] AllowedUrls { get; set; } = new string[0];
        
        /// <summary>
        /// Settings for generate JWT
        /// </summary>
        public AuthSettings AuthSettings { get; set; } = new AuthSettings();
    }
}