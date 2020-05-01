namespace Corvette.Chat.Services.Exceptions
{
    /// <summary>
    /// The exception that is thrown when action is forbidden for a user.
    /// </summary>
    public class ForbiddenException : ChatServiceException
    {
        public ForbiddenException(string message) : base(message)
        {
        }
    }
}