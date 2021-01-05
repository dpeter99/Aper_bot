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

namespace Aper_bot.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users => Set<User>();

        public DbSet<Guild> Guilds => Set<Guild>();

        public DbSet<Quote> Quotes => Set<Quote>();

        IOptions<DatabaseSettings> settings;

        public DatabaseContext(IOptions<DatabaseSettings> options)
        {
            settings = options;
            //Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            

            var connectionString = $"server={settings.Value.Address};port=3306;user={settings.Value.User};password={settings.Value.Password};database={settings.Value.Database_Name}";

            optionsBuilder.UseMySql(
                    connectionString,
                    MariaDbServerVersion.FromString("10.4.12-MariaDB-1:10.4.12+maria~bionic"),
                    MysqlOptions)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors();

            
            
        }

        void MysqlOptions(MySqlDbContextOptionsBuilder options)
        {
            options.CharSetBehavior(CharSetBehavior.NeverAppend);
            //options.MigrationsAssembly("Aper_bot.Database.Migrations");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<User>(entity =>
                {
                    entity.Property(e => e.Name)
                        .HasMaxLength(255);

                    /*
                    entity.HasData(new User[]
                    {
                        new User{Name="Test"}
                    });
                    */
                });

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

    }
}
