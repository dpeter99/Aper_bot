using Aper_bot.Database;
using Aper_bot.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Options;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace Aper_bot.Hosting.Database
{
    public class DatabaseContext : SchemaAwareDbContext
    {
        //private readonly IOptions<DatabaseSettings> _options;

        public DatabaseContext(string schema, DbContextOptions options) : base(schema, options)
        {
            //_options = options;
            
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString =
                    //    $"server={_options.Value.Address};port=3306;user={_options.Value.User};password={_options.Value.Password};database={_options.Value.Database_Name}";
                    @"THE CONNECTION STRING";

                optionsBuilder.UseMySql(
                    connectionString,
                    MariaDbServerVersion.Parse("10.4.12-MariaDB-1:10.4.12+maria~bionic"),
                    MysqlOptions);

                //.EnableSensitiveDataLogging()
                //.EnableDetailedErrors();
            }
        }
        
        
        void MysqlOptions(MySqlDbContextOptionsBuilder options)
        {
            //options.CharSetBehavior(CharSetBehavior.NeverAppend);
            options.SchemaBehavior(MySqlSchemaBehavior.Translate, Translator);
            options.MigrationsHistoryTable(Translator(_schema, HistoryRepository.DefaultTableName));
        }

        private string Translator(string schemaname, string objectname)
        {
            return $"{schemaname}.{objectname}";
        }
    }
}