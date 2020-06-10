namespace Corvette.Chat.WebService.Models
{
    /// <summary>
    /// Model of business logic error
    /// </summary>
    public class ErrorModel
    {
        /// <summary>
        /// Description of business logic error
        /// </summary>
        public string Description { get; }
        
        /// <summary>
        /// Model key
        /// </summary>
        public string? Key { get; }

        public ErrorModel(string description, string? key)
        {
            Description = description;
            Key = key;
        }
    }
}