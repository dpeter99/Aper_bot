using Aper_bot.Database.Model;

using DSharpPlus.EventArgs;

using System.Threading.Tasks;
using System.Linq;

namespace Aper_bot.Modules.Commands
{
    internal class CommandSourceStack
    {
        public MessageCreateEventArgs @event;

        public Task? exectutionTask;

        public Guild? guild { get; private set; }

        public CommandSourceStack(MessageCreateEventArgs @event, Microsoft.EntityFrameworkCore.IDbContextFactory<Database.DatabaseContext> dbContextFactory)
        {
            this.@event = @event;

            using (var db = dbContextFactory.CreateDbContext())
            {
                guild = (from g in db.Guilds
                        where g.GuildID == @event.Guild.Id.ToString()
                        select g).FirstOrDefault();

                
            }
            
        }
    }
}