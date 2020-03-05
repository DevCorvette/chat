namespace Corvette.Chat.Data
{
    /// <summary>
    /// Factory which creates <see cref="ChatDataContext"/>.
    /// </summary>
    public interface IChatDataContextFactory
    {
        /// <summary>
        /// Returns a new <see cref="ChatDataContext"/>.
        /// </summary>
        ChatDataContext CreateContext();
    }
    
    /// <inheritdoc/>
    public sealed class ChatDataContextFactory : IChatDataContextFactory
    {
        private readonly string _connectionString;
        
        public ChatDataContextFactory(string connectionString)
        {
            _connectionString = connectionString;
        }
        
        /// <inheritdoc/>
        public ChatDataContext CreateContext()
        {
            return new ChatDataContext(_connectionString);
        }
    }
}