using System.Collections.Generic;
using Aper_bot.Events;
using Aper_bot.Modules.CommandProcessing;
using Aper_bot.Modules.CommandProcessing.Attributes;
using Mars;

namespace Aper_bot.Modules.Commands
{
    [CommandProvider]
    public class PingCommand : ChatCommands
    {
        public override IEnumerable<CommandNode> Register()
        {
            var great = new Mars.LiteralNode("ping", new CommandMetaData(2));
            great.ThisCalls( new CommandFunction(Run));

            return new[] {great};
        }

        private int Run(ParseResult result, IMessageCreatedEvent discordmessageevent)
        {
            discordmessageevent.Respond("pong!");
            return 0;
        }
    }
}