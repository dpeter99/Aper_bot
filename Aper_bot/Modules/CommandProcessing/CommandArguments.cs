using System.Threading.Tasks;
using Aper_bot.Events;
using Brigadier.NET.Context;

namespace Aper_bot.Modules.CommandProcessing
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
