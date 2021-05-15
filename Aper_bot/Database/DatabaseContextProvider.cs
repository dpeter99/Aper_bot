using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
namespace Aper_bot.Database
{
    [Obsolete]
    class DatabaseContextProvider: IDbContextFactory<CoreDatabaseContext>
    {
        private IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="JellyfinDbProvider"/> class.
        /// </summary>
        /// <param name="serviceProvider">The application's service provider.</param>
        /// <param name="appPaths">The application paths.</param>
        public DatabaseContextProvider(IServiceProvider serviceProvider, IHostEnvironment environment)
        {
            _serviceProvider = serviceProvider;

            using var jellyfinDb = CreateDbContext();
            //Console.WriteLine($"THIS IS : {environment.EnvironmentName} NOT PATRIC");
            if(environment.EnvironmentName != "Design")
            {
                //jellyfinDb.Database.Migrate();
            }
            
        }

        public CoreDatabaseContext CreateDbContext()
        {
            return ActivatorUtilities.CreateInstance<CoreDatabaseContext>(_serviceProvider);
        }
    }
}
*/