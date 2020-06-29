using System;

namespace Corvette.Chat.Logic.Exceptions
{
    /// <summary>
    /// Base exception for each exception of the chat business logic layer.
    /// </summary>
    public class ChatLogicException : Exception
    {
        public ChatLogicException(string message) : base(message)
        {
        }
    }
}