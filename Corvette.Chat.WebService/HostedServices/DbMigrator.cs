using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Corvette.Chat.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Corvette.Chat.WebService.HostedServices
{
    /// <summary>
    /// Migrates chat data base
    /// </summary>
    public class DbMigrator : IHostedService
    {
        private readonly IChatDataContextFactory _contextFactory;

        private readonly ILogger<DbMigrator> _logger;

        public DbMigrator(IChatDataContextFactory contextFactory, ILogger<DbMigrator> logger)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task StartAsync(CancellationToken cToken)
        {
            _logger.LogInformation("Migration started");
            await using var context = _contextFactory.CreateContext();

            var migrations = (await context.Database.GetPendingMigrationsAsync(cToken)).ToList();
            if (migrations.Count > 0)
            {
                _logger.LogInformation($"Was found {migrations.Count} migrations which awaiting apply: {string.Join("\n", migrations)}");
                
                await context.Database.MigrateAsync(cToken);
                
                _logger.LogInformation("All migrations successfully applied.");
            }
            else
            {
                _logger.LogInformation("Wasn't found any pending migrations.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}