using Aper_bot.Hosting;
using Aper_bot.Modules.Discord.SlashCommands;
using Extensions.Hosting.AsyncInitialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Aper_bot.Modules.CommandProcessing
{
    [Module("CommandProcessing")]
    public class CommandProcessingModule : IModule
    {
        public void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<ICommandHandler,CommandHandler>();
            services.AddSingleton<IAsyncInitializer, CommandHandler>(
                serviceProvider => ((CommandHandler) serviceProvider.GetService<ICommandHandler>())!);
            
            services.AddSingleton<ISlashCommandSuplier,BrigadierSlashCommandSuplier>();
        }
    }
}