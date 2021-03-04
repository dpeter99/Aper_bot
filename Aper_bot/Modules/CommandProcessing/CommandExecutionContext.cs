using System.Threading.Tasks;
using Aper_bot.Database;
using Aper_bot.Events;
using Brigadier.NET.Context;

namespace Aper_bot.Modules.CommandProcessing
{
    /// <summary>
    /// This holds the event that triggered the command execution and the Command delegate to execute 
    /// </summary>
    public class CommandExecutionContext
    {
        public IMessageCreatedEvent Event;

        public Command? command;

        public CommandContext<CommandExecutionContext>? ctx;

        public CoreDatabaseContext db;

        public delegate Task Command(CommandContext<CommandExecutionContext> ctx, IMessageCreatedEvent discordMessageEvent);

        public CommandExecutionContext(IMessageCreatedEvent @event)
        {
            Event = @event;
        }
    }
}
