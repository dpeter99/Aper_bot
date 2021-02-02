using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Aper_bot.Database;
using Aper_bot.EventBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Aper_bot.Hosting
{
    public class APCHost : IApplicationHost
    {
        private readonly string[] _args;
        private IHost _host;

        private Dictionary<string, IModule> _modules = new Dictionary<string, IModule>();

        private ILogger _logger;
        
        public APCHost(string[] args)
        {
            static void UnhandledExceptionToConsole(object sender, UnhandledExceptionEventArgs e) =>
                Console.Error.WriteLine("Unhandled Exception\n" + e.ExceptionObject.ToString());
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionToConsole;
            
            _args = args;
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console(
                    outputTemplate:"[{Timestamp:HH:mm:ss} {SourceContext} {Level:u3}] {Message:lj}{NewLine}{Exception}"
                    )
                .CreateLogger();

            _logger = new SerilogLoggerFactory().CreateLogger("Main");
            
            AppDomain.CurrentDomain.UnhandledException -= UnhandledExceptionToConsole;
            AppDomain.CurrentDomain.UnhandledException += (sender, e)
                => _logger.LogCritical((Exception)e.ExceptionObject, "Unhandled Exception");
        }
        
        public void Init()
        {
            RegisterModules();
            
            
            //Build the Host
            var builder = Host.CreateDefaultBuilder(_args);
            //builder.ConfigureContainer<Service>();
            builder.ConfigureLogging((c, l) =>
            {
                l.ClearProviders();
                l.AddSerilog();
            });
            
            builder.ConfigureServices( RegisterServices);

            _host = builder.Build();
        }


        public void Run()
        {
            _host.InitAsync();
            _host.Run();
        }

        private void RegisterModules()
        {
            //Add the modules to the list.
            var modules = Assembly.GetExecutingAssembly().DefinedTypes.Where(e => e.CustomAttributes.Any(a => a.AttributeType == typeof(ModuleAttribute)));
            foreach (var module in modules)
            {
                if (module.ImplementedInterfaces.Any(i=>i == typeof(IModule)))
                {
                    ModuleAttribute? attr = (ModuleAttribute) Attribute.GetCustomAttribute(module, typeof (ModuleAttribute))!;
                    var name = attr._name;
                    
                    var ctor = module.DeclaredConstructors.Where(c => c.IsPublic && c.GetParameters().Length == 0);
                    var res = (IModule)ctor.FirstOrDefault()?.Invoke(null)!;
                    _modules.Add(name, res);
                }
            }
        }

        private void RegisterServices(HostBuilderContext ctx, IServiceCollection services)
        {
            RegisterDefaultServices(ctx, services);
            
            //Register services for each module
            foreach (var module in _modules)
            {
                module.Value.RegisterServices(services);
            }
        }

        private void RegisterDefaultServices(HostBuilderContext ctx, IServiceCollection services)
        {
            services.AddSingleton(Log.Logger);
            services.AddSingleton<IEventBus>(new EventBus.EventBus());
            
            services.Configure<Config>(ctx.Configuration.GetSection("Config"));
            services.Configure<DatabaseSettings>(ctx.Configuration.GetSection("Database"));
            
            services.AddSingleton<IDbContextFactory<DatabaseContext>, DatabaseContextProvider>();

            services.AddHttpClient();
        }
    }
}