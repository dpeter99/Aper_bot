using Aper_bot.Modules.Commands.DiscordArguments;

using Brigadier.NET;
using Brigadier.NET.Builder;
using Brigadier.NET.Context;

using Serilog;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aper_bot.Modules.Commands
{
    class BasicCommand: ChatCommand
    {
        ILogger Logger;

        public BasicCommand(CommandHandler commandHandler, ILogger logger)
        {
            Logger = logger;
            //commandHandler.dispatcher.Register(Register);
        }

        public override LiteralArgumentBuilder<CommandSourceStack> Register(IArgumentContext<CommandSourceStack> l)
        {
            return l.Literal("/greet")
                        .Then(a =>
                            a.Argument("user", DiscordArgumentTypes.User())
                                .Executes(Run)
                        )
                        .Then(a=>
                            a.Literal("set")
                                .Then(a=>a.Argument("value",Arguments.Integer())
                                .Executes(Set)
                        ))
                        .Executes(c =>
                        {
                            Console.WriteLine("Called foo with no arguments");
                            return 1;
                        });
        }

        private int Run(CommandContext<CommandSourceStack> context)
        {
            string messageTemplate = "Hi! " + DiscordArgumentTypes.GetUser(context, "user").Username + " <a:bolbreach:780085145079119873>";
            Logger.Information(messageTemplate);
            context.Source.@event.Message.RespondAsync(messageTemplate);
            return 1;
        }

        private int Set(CommandContext<CommandSourceStack> context)
        {
            string messageTemplate = "Foo is set to: " + Arguments.GetInteger(context, "value");
            Logger.Information(messageTemplate);
            context.Source.@event.Message.RespondAsync(messageTemplate);
            return 1;
        }
    }
}
