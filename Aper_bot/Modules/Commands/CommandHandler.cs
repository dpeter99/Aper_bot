using Aper_bot.EventBus;

using Brigadier.NET;
using Brigadier.NET.Exceptions;

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
        Serilog.ILogger logger;

        public CommandHandler(IEventBus bus, IServiceProvider provider, Serilog.ILogger log)
        {
            eventBus = bus;
            logger = log;

            

            var command = ActivatorUtilities.CreateInstance<BasicCommand>(provider, new object[] { this });
            dispatcher.Register(command.Register);
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
            if (message.Message.Content[0] != '/')
            {
                return;
            }

            var context = new CommandSourceStack(message);



            try
            {
                var res = dispatcher.Parse(message.Message.Content, context);

                if (res.Exceptions.Count <= 0)
                {
                    dispatcher.Execute(res);
                }
                else
                {
                    string text = "";

                    CommandSyntaxException exc = res.Exceptions.First().Value;
                    text = exc.InnerException?.Message ?? exc.Message;

                    message.Message.RespondAsync(text);   
                }

            }
            catch (Exception e)
            {
                logger.Debug(e, "Error in parsing command");
                if (e is CommandSyntaxException error)
                {
                    message.Message.RespondAsync(error.Message);
                }

            }



        }
    }
}
