using Aper_bot.Database;
using Aper_bot.Database.Model;
using Aper_bot.Hosting.Database;
using Aper_bot.Modules.CommandProcessing;
using Aper_bot.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

using Microsoft.Extensions.Options;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Aper_bot.Modules.DiscordSlash.Database
{
    public class SlashDbContext: DatabaseContext
    {
        public DbSet<Model.Command> Commands => Set<Model.Command>();
        
        IOptions<DatabaseSettings> settings;

        public SlashDbContext(IOptions<DatabaseSettings> options) : base("SlashCommands", options)
        {
            settings = options;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SlashDbContext).Assembly);
            
        }
    }
}