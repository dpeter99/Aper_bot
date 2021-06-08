using System.Collections.Generic;
using System.Threading.Tasks;
using Extensions.Hosting.AsyncInitialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Aper_bot.Hosting.Database
{
    public class DatabaseMigrator: IAsyncInitializer
    {
        private readonly IEnumerable<IMigrationContext> _migrationContexts;
        private readonly IHostEnvironment _environment;
        private readonly ILogger<DatabaseMigrator> _logger;

        public DatabaseMigrator(IEnumerable<IMigrationContext> migrationContexts, IHostEnvironment environment, ILogger<DatabaseMigrator> logger)
        {
            _migrationContexts = migrationContexts;
            _environment = environment;
            _logger = logger;
        }


        public async Task InitializeAsync()
        {
            if(_environment.EnvironmentName == "Design")
            {
                return;
            }
            
            foreach (var migrationContext in _migrationContexts)
            {
                _logger.LogInformation("Migrating: {Database}", migrationContext.GetContext().GetType().Name);
                await migrationContext.GetContext().Database.MigrateAsync();
            }
        }
    }

    public interface IMigrationContext
    {
        DbContext GetContext();
    }
    
    public class MigrationContext<T>: IMigrationContext  where T : DbContext 
    {
        private readonly T dbContext;
        
        public MigrationContext(T factory)
        {
            dbContext = factory;
        }
        
        public DbContext GetContext()
        {
            return dbContext;
        }
    }
}