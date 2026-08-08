using System;
using Curiosity.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
        ChatDataContext CreateContext(bool isLoggingEnabled = false);
    }

    public sealed class ChatDataContextFactory : ICuriosityDataContextFactory<ChatDataContext>, IChatDataContextFactory
    {
        private readonly DbOptions _dbOptions;
        private readonly ILoggerFactory _loggerFactory;

        public ChatDataContextFactory(DbOptions dbOptions, ILoggerFactory loggerFactory)
        {
            _dbOptions = dbOptions ?? throw new ArgumentNullException(nameof(dbOptions));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public ChatDataContext CreateContext(bool isLoggingEnabled = false)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ChatDataContext>()
                .UseNpgsql(_dbOptions.ConnectionString) 
                .UseLazyLoadingProxies();               
            
            if (_dbOptions.IsGlobalLoggingEnabled || isLoggingEnabled)
            {
                optionsBuilder.UseLoggerFactory(_loggerFactory);

                if (_dbOptions.IsSensitiveDataLoggingEnabled)
                    optionsBuilder.EnableSensitiveDataLogging();
            }
            
            return new ChatDataContext(optionsBuilder.Options);
        }

        ICuriosityDataContext ICuriosityDataContextFactory.CreateContext(bool isLoggingEnabled)
        {
            return CreateContext(isLoggingEnabled);
        }
    }
}