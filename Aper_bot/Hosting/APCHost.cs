using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using Aper_bot.Database;
using Aper_bot.EventBus;
using Aper_bot.Hosting.Database;
using Aper_bot.Hosting.WebHost;
using Aper_bot.Util;
using Aper_bot.Util.Singleton;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Aper_bot.Hosting
{
    public class APCHost : Singleton<APCHost>, IApplicationHost
    {
        private readonly string[] _args;
        private IHost? _host;

        private Dictionary<string, IModule> _modules = new Dictionary<string, IModule>();

        private readonly ILogger _logger;

        public IServiceProvider? Services => _host?.Services;
        
        public APCHost(string[] args)
        {
            Console.WriteLine("Aper Bot is starting up");
            
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
            
            _logger.LogInformation(
                "Aper bot version: {Version}",
                Assembly.GetEntryAssembly()!.GetName().Version!.ToString(3));
            
            AppDomain.CurrentDomain.UnhandledException -= UnhandledExceptionToConsole;
            AppDomain.CurrentDomain.UnhandledException += (sender, e)
                => _logger.LogCritical((Exception)e.ExceptionObject, "Unhandled Exception");
        }
        
        public void Init()
        {
            RegisterModules();
            
            _logger.LogInformation(
                "Found modules:\n" +
                "{Modules}",
                String.Join("\n",_modules.Select(m => m.Key)));
            
            
            //Build the Host
            var builder = Host.CreateDefaultBuilder(_args);

            if (_modules.Any(m => m.Value.IsAspRequiered()))
                builder.ConfigureWebHost(WebHostConfig);

            
             
            
            builder.ConfigureLogging((context, l) =>
            {
                l.ClearProviders();
                
                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(context.Configuration)
                    .Enrich.FromLogContext()
                    .WriteTo.Console(
                        outputTemplate:"[{Timestamp:HH:mm:ss} {SourceContext} {Level:u3}] {Message:lj}{NewLine}{Exception}"
                    )
                    .CreateLogger();
                
                l.AddSerilog();
            });

            builder.ConfigureServices(RegisterServices);

            _host = builder.Build();
        }

        private void WebHostConfig(IWebHostBuilder builder)
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


        public void Run()
        {
            if (_host != null)
            {
                
                _host.InitAsync();
                _host.Run();
            }
            else
            {
                _logger.LogCritical("There is no host to run. The host building might have failed");
            }
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
                module.Value.RegisterServices(ctx,services);
            }
        }

        private void RegisterDefaultServices(HostBuilderContext ctx, IServiceCollection services)
        {
            //services.AddAsyncInitialization();
            
            //services.AddSingleton(Log.Logger);
            //services.AddSingleton<IEventBus>(new EventBus.EventBus());
            
            services.Configure<Config>(ctx.Configuration.GetSection("Config"));
            services.Configure<DatabaseSettings>(ctx.Configuration.GetSection("Database"));
            
            //services.Configure<HostingConfig>(ctx.Configuration.GetSection("Hosting"));
            //services.Configure<CertConfig>(ctx.Configuration.GetSection("Hosting:CertConfig"));

            //services.AddAsyncInitializer<DatabaseMigrator>();
            
            //services.AddSingleton<IDbContextFactory<CoreDatabaseContext>, DatabaseContextProvider>();
            //services.AddSingleton<DatabaseContextProvider>();
            //services.AddDbContextFactory<CoreDatabaseContext,DatabaseContextProvider>(lifetime: ServiceLifetime.Singleton);
            //services.AddDbContextFactory<CoreDatabaseContext>();
            
            //var ctrString = $"server={ctx.Configuration["Database:Address"]};port=3306;user={ctx.Configuration["Database:User"]};password={ctx.Configuration["Database:Password"]};database={ctx.Configuration["Database:Database_Name"]}";
            
            services.AddDbContext<CoreDatabaseContext>();
            //services.AddTransient<IMigrationContext,MigrationContext<CoreDatabaseContext>>();

            //services.AddHttpClient();

            services.AddTransient<IHostedService, TestManyServiceProviders>();
        }
    }
}