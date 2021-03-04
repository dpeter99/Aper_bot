using Aper_bot.Hosting;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Aper_bot.Modules.Discord
{
    [Module(name:"DiscordBot")]
    public class DiscordBotModule: IModule
    {
        public void RegisterServices(HostBuilderContext ctx,IServiceCollection services)
        {
            services.AddSingleton<DiscordBot>();
            services.AddSingleton<IHostedService, DiscordBot>(
                serviceProvider => serviceProvider.GetService<DiscordBot>()!);
        }
    }
}