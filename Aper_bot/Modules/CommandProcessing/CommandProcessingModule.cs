using System.Linq;
using System.Reflection;
using Aper_bot.Hosting;
using Aper_bot.Modules.CommandProcessing.Attributes;
using Aper_bot.Modules.CommandProcessing.Commands;
using Aper_bot.Modules.DiscordSlash;
using Extensions.Hosting.AsyncInitialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Aper_bot.Modules.CommandProcessing
{
    [Module("CommandProcessing")]
    public class CommandProcessingModule : IModule
    {
        public void RegisterServices(HostBuilderContext ctx,IServiceCollection services)
        {
            services.Configure<CommandBaseConfig>(ctx.Configuration.GetSection("CommandsConfig"));
            
            services.AddSingleton<ICommandProcessor,CommandProcessor>();
            services.AddSingleton<IAsyncInitializer>(
                serviceProvider => ((CommandProcessor) serviceProvider.GetService<ICommandProcessor>())!);
            
            //services.AddSingleton<ISlashCommandSuplier,BrigadierSlashCommandSuplier>();
            
            var commands = Assembly.GetExecutingAssembly().DefinedTypes.Where(e => e.CustomAttributes.Any(a => a.AttributeType == typeof(CommandProviderAttribute)));
            foreach (var item in commands)
            {
                if (item.IsSubclassOf(typeof(ChatCommands)))
                {
                    services.AddSingleton(typeof(ChatCommands), item);
                }
            }
        }
    }
}