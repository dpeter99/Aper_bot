using System.Linq;
using System.Threading.Tasks;
using Aper_bot.Database;
using Aper_bot.Database.Model;
using Aper_bot.Events;
using Aper_bot.Modules.CommandProcessing;
using Aper_bot.Modules.CommandProcessing.Attributes;
using Aper_bot.Modules.CommandProcessing.DiscordArguments;
using Brigadier.NET.Builder;
using Brigadier.NET.Context;
using Microsoft.EntityFrameworkCore;

namespace Aper_bot.Command
{
    [CommandProvider]
    class UserCommands : ChatCommands
    {
        Serilog.ILogger logger;

        //IDbContextFactory<DatabaseContext> dbContextFactory;

        public UserCommands(Serilog.ILogger log, IDbContextFactory<DatabaseContext> fac)
        {
            logger = log;
            //dbContextFactory = fac;
        }

        public override LiteralArgumentBuilder<CommandArguments> Register(IArgumentContext<CommandArguments> l)
        {
            return l.Literal("/user")
                    .Then(
                        a =>
                        a.Literal("info")
                            .Then(
                                i =>
                                    i.Argument("user", DiscordArgumentTypes.User())
                                     .Executes(AsyncExecue(User))
                                )


                    )
                    .Then(
                        a =>
                        a.Argument("user", DiscordArgumentTypes.User())
                         .Executes(AsyncExecue(User))
                    );

        }

        public Task User(CommandContext<CommandArguments> ctx, MessageCreatedEvent messageEvent)
        {

            DSharpPlus.EventArgs.MessageCreateEventArgs @event = messageEvent.@event;
            var user = (from u in messageEvent.db.Users
                        where u.Name == @event.Author.Username
                        select u)
                       .FirstOrDefault();

            if (user != null)
            {
                @event.Message.RespondAsync($"{user.Name} is a nice fellow");
            }
            else
            {
                messageEvent.db.Add(new User(@event.Author));
                messageEvent.db.SaveChangesAsync().Wait();
                @event.Message.RespondAsync($"I have never see this person. Gona remember {@event.Author.Username}.");
            }

            return Task.CompletedTask;
        }

        public int UserInfo(CommandContext<CommandArguments> ctx, MessageCreatedEvent messageEvent)
        {

            DSharpPlus.EventArgs.MessageCreateEventArgs @event = messageEvent.@event;
            var user = (from u in messageEvent.db.Users
                        where u.Name == @event.Author.Username
                        select u)
                       .FirstOrDefault();

            if (user != null)
            {
                @event.Message.RespondAsync($"{user.Name} is a nice fellow");
            }
            else
            {
                messageEvent.db.Add(new User(@event.Author));
                messageEvent.db.SaveChangesAsync().Wait();
                @event.Message.RespondAsync($"I have never see this person. Gona remember {@event.Author.Username}.");
            }

            return 0;
        }
    }
}
