namespace Corvette.Chat.Logic.Exceptions
{
    /// <summary>
    /// The exception that is thrown when action is forbidden for a user.
    /// </summary>
    public class ForbiddenException : ChatLogicException
    {
        public ForbiddenException(string message) : base(message)
        {
        }
    }
}