using Aper_bot.Modules.CommandProcessing;
using Aper_bot.Modules.CommandProcessing.Attributes;
using Aper_bot.Modules.CommandProcessing.DiscordArguments;
using Brigadier.NET.Builder;
using Brigadier.NET.Context;
using Serilog;

namespace Aper_bot.Commands
{
    [CommandProvider]
    class BasicCommand: ChatCommands
    {
        ILogger Logger;

        public BasicCommand(ILogger logger)
        {
            Logger = logger;
            //commandHandler.dispatcher.Register(Register);
        }

        public override LiteralArgumentBuilder<CommandExecutionContext> Register(IArgumentContext<CommandExecutionContext> l)
        {
            return l.Literal("greet", "Just a nice great ...")
                        .Then(a =>
                            a.Argument("user", DiscordArgumentTypes.User())
                                .Executes(Run)
                        );
        }

        private int Run(CommandContext<CommandExecutionContext> context)
        {
            string messageTemplate = "Hi! " + DiscordArgumentTypes.GetUser(context, "user").Username + " <a:bolbreach:780085145079119873>";
            Logger.Information(messageTemplate);
            //context.Source.Event.@event.Message.RespondAsync(messageTemplate);

            context.Source.Event.Respond(messageTemplate);
            
            return 1;
        }

    }
}
