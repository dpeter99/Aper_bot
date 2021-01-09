using Aper_bot.Events;

using Brigadier.NET.Context;

using System.Threading.Tasks;

namespace Aper_bot.Modules.Commands
{
    public class CommandArguments
    {
        public MessageCreatedEvent Event;

        public Command? exectutionTask;

        public CommandContext<CommandArguments>? ctx;

        public delegate Task Command(CommandContext<CommandArguments> ctx, MessageCreatedEvent messageEvent);

        public CommandArguments(MessageCreatedEvent @event)
        {
            Event = @event;
        }
    }
}
