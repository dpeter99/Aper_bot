using Aper_bot.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Aper_bot.Modules.WebAPI
{
    [Module("Web API")]
    public class WebAPIModule : IModule
    {
        public void RegisterServices(HostBuilderContext ctx, IServiceCollection services)
        {
            
        }
        
        public bool IsAspRequired()
        {
            return true;
        }
    }
}