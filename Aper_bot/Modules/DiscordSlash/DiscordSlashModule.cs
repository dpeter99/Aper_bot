using Aper_bot.Database;
using Aper_bot.Hosting;
using Aper_bot.Hosting.Database;
using Aper_bot.Modules.DiscordSlash.Database;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Aper_bot.Modules.DiscordSlash
{
    //[Module("Discord Slash")]
    public class DiscordSlashModule: IModule
    {
        
        public void RegisterServices(HostBuilderContext ctx,IServiceCollection services)
        {
            services.AddSingleton<SlashCommandHandler>();
            //services.AddAsyncInitializer(
            //    serviceProvider => ((SlashCommandHandler) serviceProvider.GetService<SlashCommandHandler>())!);
            
            //services.AddSingleton<IHostedService, SlashCommandHandler>(
            //    serviceProvider => ((SlashCommandHandler) serviceProvider.GetService<SlashCommandHandler>())!);

            //services.AddDbContext<SlashDbContext>();
            services.AddDbContextFactory<SlashDbContext>();
            services.AddScoped<IMigrationContext, MigrationContext<SlashDbContext>>();
        }

        public bool IsAspRequiered()
        {
            return true;
        }
    }
}