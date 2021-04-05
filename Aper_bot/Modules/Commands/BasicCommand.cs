using System.Collections.Generic;
using Aper_bot.Events;
using Aper_bot.Modules.CommandProcessing;
using Aper_bot.Modules.CommandProcessing.Attributes;
using Aper_bot.Modules.CommandProcessing.DiscordArguments;
using DSharpPlus.Entities;
using Mars;
using Mars.Arguments;
using Serilog;

namespace Aper_bot.Modules.Commands
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

        public override IEnumerable<CommandNode> Register()
        {
            var great = new Mars.LiteralNode("greet");
            great.NextArgument("user", DiscordArgumentTypes.User()).ThisCalls( new CommandFunction(Run));

            return new[] {great};
        }

        private int Run(ParseResult result, IMessageCreatedEvent context)
        {
            /*
            string messageTemplate = "Hi! " + DiscordArgumentTypes.GetUser(context, "user").Username + " <a:bolbreach:780085145079119873>";
            Logger.Information(messageTemplate);
            //context.Source.Event.@event.Message.RespondAsync(messageTemplate);

            context.Source.Event.Respond(messageTemplate);
            */
            DiscordUser u = (DiscordUser) result.Args["user"];
            
            string messageTemplate = "Hi! " + u.Username + " <a:bolbreach:780085145079119873>";
            
            context.RespondBasic(messageTemplate);
            
            return 1;
        }

    }
}
