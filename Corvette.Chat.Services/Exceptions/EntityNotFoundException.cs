namespace Corvette.Chat.Services.Exceptions
{
    /// <summary>
    /// The exception that is thrown when an entity was not found in the database.
    /// </summary>
    public sealed class EntityNotFoundException : ChatServiceException
    {
        public EntityNotFoundException(string message) : base(message)
        {
        }
    }
}