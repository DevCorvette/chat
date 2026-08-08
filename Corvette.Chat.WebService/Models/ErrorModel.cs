using System.Text.Json;

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
        public string? Description { get; set; }
        
        /// <summary>
        /// Model key
        /// </summary>
        public string? Key { get; set; }

        public ErrorModel(string description, string? key)
        {
            Description = description;
            Key = key;
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions{WriteIndented = true});
        }
    }
}