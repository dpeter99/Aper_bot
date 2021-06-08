using System;
using Aper_bot.Database;
using Aper_bot.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace Aper_bot.Hosting.Database
{
    public class DatabaseContext : SchemaAwareDbContext
    {
        private readonly IOptions<DatabaseSettings> _options;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IHostEnvironment _env;

        public DatabaseContext(string schema, IOptions<DatabaseSettings> options, ILoggerFactory loggerFactory, IHostEnvironment env) : base(schema)
        {
            _options = options;
            _loggerFactory = loggerFactory;
            _env = env;
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString =
                    $"server={_options.Value.Address};port=3306;user={_options.Value.User};password={_options.Value.Password};database={_options.Value.Database_Name}";
                    

                optionsBuilder.UseMySql(
                    connectionString,
                    MariaDbServerVersion.Parse("10.4.12-MariaDB-1:10.4.12+maria~bionic"),
                    MysqlOptions)
                    .UseLoggerFactory(_loggerFactory)
                    .EnableDetailedErrors();

                if (_env.IsDevelopment())
                {
                    optionsBuilder.EnableSensitiveDataLogging();
                }
                
            }
        }
        
        
        void MysqlOptions(MySqlDbContextOptionsBuilder options)
        {
            //options.CharSetBehavior(CharSetBehavior.NeverAppend);
            options.SchemaBehavior(MySqlSchemaBehavior.Translate, Translator);
            options.MigrationsHistoryTable(Translator(_schema, HistoryRepository.DefaultTableName));
            
            
        }

        private static string Translator(string schemaname, string objectname)
        {
            return $"{schemaname}.{objectname}";
        }
    }
}