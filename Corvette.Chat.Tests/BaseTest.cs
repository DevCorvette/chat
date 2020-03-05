using Corvette.Chat.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace Corvette.Chat.Tests
{
    public class BaseTest
    {
        private readonly DbContextOptions<ChatDataContext> _options;

        protected BaseTest()
        {
            var builder = new DbContextOptionsBuilder<ChatDataContext>();
            builder.UseInMemoryDatabase($"{GetType().Name}_database", new InMemoryDatabaseRoot());
            _options = builder.Options;
            
            var dataContext = new ChatDataContext(_options, true);
            dataContext.Database.EnsureDeleted();
            dataContext.Database.EnsureCreated();
        }

        /// <summary>
        /// Create a new data context that uses the "In memory" database.
        /// </summary>
        protected ChatDataContext CreateContext()
        {
            return new ChatDataContext(_options, true);
        }

        /// <summary>
        /// Returns service collection with context factory, logger, and target service.
        /// You can add in it other mock services that need for tests.
        /// </summary>
        protected IServiceCollection GetServiceCollection<TInterface, TImplementation>() 
            where TInterface : class 
            where TImplementation : class, TInterface
        {
            var services = new ServiceCollection();

            var factoryMock = new Mock<IChatDataContextFactory>();
            factoryMock.Setup(x => x.CreateContext())
                .Returns(CreateContext);

            services
                .AddSingleton(factoryMock.Object)
                .AddSingleton(Mock.Of<ILogger<TImplementation>>())
                .AddSingleton<TInterface, TImplementation>();

            return services;
        }
    }
}