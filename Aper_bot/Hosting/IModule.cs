using Microsoft.Extensions.DependencyInjection;

namespace Aper_bot.Hosting
{
    public interface IModule
    {
        void RegisterServices(IServiceCollection services);
    }
}