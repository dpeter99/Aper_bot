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
    class BasicCommand: ChatCommands
    {
        ILogger Logger;

        public BasicCommand(ILogger logger)
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
                        );
        }

        private int Run(CommandContext<CommandSourceStack> context)
        {
            string messageTemplate = "Hi! " + DiscordArgumentTypes.GetUser(context, "user").Username + " <a:bolbreach:780085145079119873>";
            Logger.Information(messageTemplate);
            context.Source.@event.Message.RespondAsync(messageTemplate);
            return 1;
        }

    }
}
