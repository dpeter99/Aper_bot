using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aper_bot.Database
{
    class DesignDatabaseFactory : IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContext CreateDbContext(string[] args)
        {
            var app = new Application(args);

            var ctx = app.host.Services.GetService<IDbContextFactory<DatabaseContext>>();
            if(ctx != null)
            {
                return ctx.CreateDbContext();
            }
            throw new Exception("Coudn't make Aper bot ctx");
        }
    }
}
