using System;
using System.Threading.Tasks;

using Aper_bot.EventBus;
using Aper_bot.Modules;
using Aper_bot.Modules.Commands;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Serilog;
using Serilog.Events;

namespace Aper_bot
{
    static class Program
    {


        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            using IHost host = CreateHostBuilder(args)
                                .Build();


            host.Run();
        }

        static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                        .ConfigureServices(Services)
                        .ConfigureLogging((c,l)=> {
                            l.ClearProviders();
                            l.AddSerilog();
                        })
                        .ConfigureHostConfiguration((config)=> {
                            config.AddEnvironmentVariables();
                        });

                        
        }

        static void Services(HostBuilderContext ctx, IServiceCollection services)
        {
            services.AddSingleton(Log.Logger);
            services.AddSingleton<IEventBus>(new EventBus.EventBus());
            services.AddHostedService<DiscordBot>();

            services.AddHostedService<CommandHandler>();
        }
    }
}
