using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Aper_bot.Hosting
{
    public interface IModule
    {
        void RegisterServices(HostBuilderContext ctx,IServiceCollection services);

        bool IsAspRequiered() => false;
    }
}