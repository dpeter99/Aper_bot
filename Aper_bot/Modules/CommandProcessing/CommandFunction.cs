using System.Threading.Tasks;
using Aper_bot.Events;

namespace Aper_bot.Modules.CommandProcessing
{
    public class CommandFunction : Mars.CommandCallback
    {
        public delegate int Command(Mars.ParseResult result,IMessageCreatedEvent discordMessageEvent);
        public delegate Task AsyncCommand(Mars.ParseResult result,IMessageCreatedEvent discordMessageEvent);

        public Command Cmd;

        public CommandFunction(Command cmd)
        {
            this.Cmd = cmd;
        }
        
        public CommandFunction(AsyncCommand cmd)
        {
            this.Cmd = ((result, @event) =>
            {
                cmd(result, @event).Wait();
                return 0;
            });
        }
    }
}