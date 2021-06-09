using System;
using System.Threading.Tasks;
using Aper_bot.Database;
using Aper_bot.Events;


namespace Aper_bot.Modules.CommandProcessing
{
    /// <summary>
    /// This holds the event that triggered the command execution and the Command delegate to execute 
    /// </summary>
    [Obsolete]
    public class CommandExecutionContext
    {
        public IMessageCreatedEvent Event;

        public Command? command;

        

        public CoreDatabaseContext Db;

        public delegate Task Command(IMessageCreatedEvent discordMessageEvent);

        public CommandExecutionContext(IMessageCreatedEvent @event)
        {
            Event = @event;
            Db = @event.Db;
        }
    }
}

