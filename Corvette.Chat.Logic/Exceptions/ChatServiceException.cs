using System;

namespace Corvette.Chat.Logic.Exceptions
{
    /// <summary>
    /// Base exception for each exception of the chat business logic layer.
    /// </summary>
    public class ChatServiceException : Exception
    {
        public ChatServiceException(string message) : base(message)
        {
        }
    }
}