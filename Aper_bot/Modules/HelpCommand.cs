using Aper_bot.EventBus;

using DSharpPlus.EventArgs;

using Microsoft.Extensions.Hosting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aper_bot.Modules
{

    class HelpCommand : IHostedService
    {
        public HelpCommand(IEventBus bus)
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
