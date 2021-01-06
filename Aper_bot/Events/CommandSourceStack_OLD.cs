using Aper_bot.Database.Model;
using Aper_bot.EventBus;

using DSharpPlus.EventArgs;

using System;
using System.Linq;
using System.Threading.Tasks;
/*
namespace Aper_bot.Modules.Commands
{
    internal class CommandSourceStack_OLD : Event
    {
        //public MessageCreateEventArgs @event;
        public EventArgs @event;

        public Task? exectutionTask;

        public Guild? guild { get; private set; }

        public User author { get; private set; }

        public Database.DatabaseContext db;

        public CommandSourceStack_OLD(EventArgs @event, Database.DatabaseContext database)
        {
            this.db = database;

            this.@event = @event;


            guild = (from g in db.Guilds
                     where g.GuildID == @event.Guild.Id.ToString()
                     select g).FirstOrDefault();

            author = db.GetOrCreateUserFor(@event.Author);

            db.SaveChangesAsync().Wait();
        

    }
}
}
*/