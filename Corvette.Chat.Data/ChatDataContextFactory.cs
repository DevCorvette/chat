namespace Corvette.Chat.Data
{
    /// <summary>
    /// Factory which creates <see cref="ChatDataContext"/>.
    /// </summary>
    public sealed class ChatDataContextFactory
    {
        private readonly string _connectionString;
        
        public ChatDataContextFactory(string connectionString)
        {
            _connectionString = connectionString;
        }
        
        /// <summary>
        /// Returns a new <see cref="ChatDataContext"/>.
        /// </summary>
        public ChatDataContext CreateContext()
        {
            return new ChatDataContext(_connectionString);
        }
    }
}