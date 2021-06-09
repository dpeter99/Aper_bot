using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aper_bot.Hosting;
using Aper_bot.Modules.DiscordSlash.Database;
using Microsoft.EntityFrameworkCore.Internal;

namespace Aper_bot.Database
{
    class DesignSlashDbContextFactory : IDesignTimeDbContextFactory<SlashDbContext>
    {
        public SlashDbContext CreateDbContext(string[] args)
        {
            var app = new APCHost(args);
            app.Init();

            var ctx = app.Services?.GetRequiredService<IDbContextFactory<SlashDbContext>>();
            if(ctx != null)
            {
                return ctx.CreateDbContext();
            }
            throw new Exception("Coudn't make Aper bot ctx");
        }
    }
}
