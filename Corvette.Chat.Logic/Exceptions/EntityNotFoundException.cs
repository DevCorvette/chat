namespace Corvette.Chat.Logic.Exceptions
{
    /// <summary>
    /// The exception that is thrown when an entity was not found in the database.
    /// </summary>
    public sealed class EntityNotFoundException : ChatLogicException
    {
        public EntityNotFoundException(string message) : base(message)
        {
        }
    }
}