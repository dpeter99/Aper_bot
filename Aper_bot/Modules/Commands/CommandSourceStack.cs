using DSharpPlus.EventArgs;

namespace Aper_bot.Modules.Commands
{
    internal class CommandSourceStack
    {
        public MessageCreateEventArgs @event;

        public CommandSourceStack(MessageCreateEventArgs @event)
        {
            this.@event = @event;
        }
    }
}