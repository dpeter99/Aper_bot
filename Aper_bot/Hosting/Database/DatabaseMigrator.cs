using System.Collections.Generic;
using System.Threading.Tasks;
using Extensions.Hosting.AsyncInitialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Aper_bot.Hosting.Database
{
    public class DatabaseMigrator: IAsyncInitializer
    {
        private readonly IEnumerable<IMigrationContext> _migrationContexts;
        private readonly IHostEnvironment _environment;

        public DatabaseMigrator(IEnumerable<IMigrationContext> migrationContexts, IHostEnvironment environment)
        {
            _migrationContexts = migrationContexts;
            _environment = environment;
        }


        public async Task InitializeAsync()
        {
            if(_environment.EnvironmentName == "Design")
            {
                return;
            }
            
            foreach (var migrationContext in _migrationContexts)
            {
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
        //private readonly IDbContextFactory<T> _factory;

        /*
        public MigrationContext(IDbContextFactory<T> factory)
        {
            _factory = factory;
        }
        */

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