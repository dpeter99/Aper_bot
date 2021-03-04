using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Aper_bot.Database;
using Aper_bot.EventBus;
using Aper_bot.Hosting.WebHost;
using Aper_bot.Modules.Discord;
using Aper_bot.Util;
using Aper_bot.Util.Singleton;
using Certes;
using FluffySpoon.AspNet.LetsEncrypt;
using FluffySpoon.AspNet.LetsEncrypt.Certes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


using Serilog;
using Serilog.Events;


namespace Aper_bot
{
    class Application: Singleton<Application>
    {
        
        public IHost host { private set; get; }

        public Application(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            static void UnhandledExceptionToConsole(object sender, UnhandledExceptionEventArgs e) =>
                Console.Error.WriteLine("Unhandled Exception\n" + e.ExceptionObject.ToString());
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionToConsole;
            
            host = CreateHostBuilder(args)
                .Build();
        }

        public void Run()
        {
            host.Run();
        }

        static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                        .ConfigureWebHostDefaults(WebHostConfig)
                        .ConfigureWebHost(WebHostConfig)
                        .ConfigureAppConfiguration(Settings)
                        .ConfigureServices(Services)
                        .ConfigureLogging((c, l) => {
                            l.ClearProviders();
                            l.AddSerilog();
                        })
                        .ConfigureHostConfiguration((config) => {
                            config.AddEnvironmentVariables();
                        })
                        
                ;
        }

        private static void WebHostConfig(IWebHostBuilder builder)
        {
            builder.UseStartup<Setup>();
            
            builder.UseKestrel((builderContext, options) =>
            {
                var appServices = options.ApplicationServices;
                
                options.Configure(builderContext.Configuration.GetSection("Kestrel"), reloadOnChange: true);
                
                options.Listen(IPAddress.Any,80);
                options.Listen(IPAddress.Any, 25580,configure: a => { a.UseHttps(); });
                
            });
        }
        
        private static void Settings(HostBuilderContext env, IConfigurationBuilder arg2)
        {
            if(env.HostingEnvironment.IsDevelopment() || env.HostingEnvironment.EnvironmentName == "Design")
            {
                arg2.AddUserSecrets<Application>();
            }
        }

        static void Services(HostBuilderContext ctx, IServiceCollection services)
        {
            //services.AddSingleton(Log.Logger);
            //services.AddSingleton<IEventBus>(new EventBus.EventBus());
            
            //services.AddSingleton<DiscordBot>();
            //services.AddSingleton<IHostedService, DiscordBot>(
            //    serviceProvider => serviceProvider.GetService<DiscordBot>()!);

            //services.AddSingleton<ICommandHandler,CommandHandler>();
            //services.AddSingleton<IHostedService, CommandHandler>(
            //    serviceProvider => ((CommandHandler) serviceProvider.GetService<ICommandHandler>())!);

            //services.AddSingleton<ISlashCommandSuplier,BrigadierSlashCommandSuplier>();
            //services.AddHostedService<SlashCommandHandler>();
            
            //services.Configure<Config>(ctx.Configuration.GetSection("Config"));
            //services.Configure<DatabaseSettings>(ctx.Configuration.GetSection("Database"));

            //services.AddSingleton<IDbContextFactory<DatabaseContext>, DatabaseContextProvider>();

            //services.AddHttpClient();
            
        }
    }
}
