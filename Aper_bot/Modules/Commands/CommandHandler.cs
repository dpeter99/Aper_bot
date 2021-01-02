using Aper_bot.EventBus;

using Brigadier.NET;

using DSharpPlus.EventArgs;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aper_bot.Modules.Commands
{
    /// <summary>
    /// This class is going to handle the incoming commands and than forward them to the right place
    /// </summary>
    class CommandHandler : IHostedService
    {
        internal CommandDispatcher<CommandSourceStack> dispatcher = new CommandDispatcher<CommandSourceStack>();

        List<ChatCommand> Commands = new List<ChatCommand>();

        IEventBus eventBus;

        public CommandHandler(IEventBus bus, IServiceProvider provider)
        {
            eventBus = bus;

            

            var command = ActivatorUtilities.CreateInstance<BasicCommand>(provider, new object[]{ this});
            Commands.Add(command);
        }



        public Task StartAsync(CancellationToken cancellationToken)
        {
            eventBus.Register(this);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }


        [EventListener()]
        public void NewMessage(MessageCreateEventArgs message)
        {
            var context = new CommandSourceStack()
            {
                @event = message
            };

            var res = dispatcher.Parse(message.Message.Content, context);
            dispatcher.Execute(res);
        }
    }
}
