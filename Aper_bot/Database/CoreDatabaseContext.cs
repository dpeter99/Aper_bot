using Aper_bot.Database.Model;

using DSharpPlus.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Options;

using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aper_bot.Hosting.Database;
using Aper_bot.Util.Discord;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Aper_bot.Database
{
    public class CoreDatabaseContext : DatabaseContext
    {
        public const string Schema = "Aper_Bot";

        public DbSet<User> Users { get; set; } // => Set<User>();

        public DbSet<Guild> Guilds { get; set; }// => Set<Guild>();

        public DbSet<Quote> Quotes { get; set; }// => Set<Quote>();
        

        public CoreDatabaseContext(IOptions<DatabaseSettings> options, ILoggerFactory loggerFactory, IHostEnvironment env):base(Schema, options, loggerFactory, env)
        {

        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<User>(entity =>
                {
                    entity.Property(e => e.Name)
                        .HasMaxLength(255);
                });

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);

        }

        public User GetOrCreateUserFor(DiscordUser discordUser)
        {
            var user = (from u in Users
                        where u.UserID == discordUser.Id.ToString()
                        select u)
                           .FirstOrDefault();

            if(user == null)
            {
               //user = ;
               user = Add(new User(discordUser.Username, discordUser.Id.ToString())).Entity;
               SaveChanges();
            }

            return user;
        }

        public Guild? GetGuildFor(DiscordGuild discordGuild)
        {
            var guild = (from u in Guilds
                        where u.GuildID == discordGuild.Id
                        select u)
                           .FirstOrDefault();

            return guild;
        }
        
        public Guild? GetGuildFor(Snowflake discordGuildId)
        {
            var guild = (from u in Guilds
                    where u.GuildID == discordGuildId
                    select u)
                .FirstOrDefault();

            return guild;
        }

    }
}
