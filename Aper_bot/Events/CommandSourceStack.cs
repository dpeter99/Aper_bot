using Aper_bot.EventBus;

using System;

namespace Aper_bot.Events
{
    public class CommandSourceStack<T> : Event where T : EventArgs
    {
        public T @event;

        public Database.DatabaseContext db;

        public CommandSourceStack(T @event, Database.DatabaseContext database)
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