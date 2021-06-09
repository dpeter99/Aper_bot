using Aper_bot.EventBus;

using System;

namespace Aper_bot.Events
{
    public class CommandSourceStack<T> : Event
    {
        public T @event;

        public Database.CoreDatabaseContext db;

        public CommandSourceStack(T @event, Database.CoreDatabaseContext database)
        {
            this.@event = @event;

            this.db = database;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            db.Dispose();
        }
    }
}