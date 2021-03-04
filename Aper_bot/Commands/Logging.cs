using System.Threading;
using System.Threading.Tasks;
using Aper_bot.EventBus;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Hosting;

namespace Aper_bot.Commands
{

    class Logging : IHostedService
    {
        public Logging(IEventBus bus)
        {
            bus.Register(this);
        }

        [EventListener()]
        public void NewMessage(MessageCreateEventArgs message)
        {
            //message.Message.RespondAsync("Hmmm");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
