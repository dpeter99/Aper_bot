using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aper_bot.Database;
using Aper_bot.Database.Model;
using Aper_bot.Events;
using Aper_bot.Modules.CommandProcessing;
using Aper_bot.Modules.CommandProcessing.Attributes;
using Aper_bot.Modules.CommandProcessing.DiscordArguments;
using Aper_bot.Modules.Discord;
using Mars;
using Microsoft.EntityFrameworkCore;

namespace Aper_bot.Modules.Commands
{
    [CommandProvider]
    class UserCommands : ChatCommands
    {
        Serilog.ILogger logger;

        //IDbContextFactory<DatabaseContext> dbContextFactory;

        public UserCommands(Serilog.ILogger log, IDbContextFactory<CoreDatabaseContext> fac)
        {
            logger = log;
            //dbContextFactory = fac;
        }

        public override IEnumerable<CommandNode> Register()
        {
            /*
            return l.Literal("user")
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
                    */
            return null;

        }

        /*
        public Task User(CommandContext<CommandExecutionContext> ctx, IMessageCreatedEvent messageEvent)
        {
            if (messageEvent is DiscordMessageCreatedEvent dmce)
            {
                
                DSharpPlus.EventArgs.MessageCreateEventArgs @event = dmce.@event;
                var user = (from u in ctx.Source.Db.Users
                        where u.Name == @event.Author.Username
                        select u)
                    .FirstOrDefault();

                if (user != null)
                {
                    @event.Message.RespondAsync($"{user.Name} is a nice fellow");
                }
                else
                {
                    ctx.Source.Db.Add(new User(@event.Author));
                    ctx.Source.Db.SaveChangesAsync().Wait();
                    @event.Message.RespondAsync($"I have never see this person. Gona remember {@event.Author.Username}.");
                }
            }

            return Task.CompletedTask;
        }

        public int UserInfo(CommandContext<CommandExecutionContext> ctx, DiscordMessageCreatedEvent discordMessageEvent)
        {

            DSharpPlus.EventArgs.MessageCreateEventArgs @event = discordMessageEvent.@event;
            var user = (from u in discordMessageEvent.db.Users
                        where u.Name == @event.Author.Username
                        select u)
                       .FirstOrDefault();

            if (user != null)
            {
                @event.Message.RespondAsync($"{user.Name} is a nice fellow");
            }
            else
            {
                discordMessageEvent.db.Add(new User(@event.Author));
                discordMessageEvent.db.SaveChangesAsync().Wait();
                @event.Message.RespondAsync($"I have never see this person. Gona remember {@event.Author.Username}.");
            }

            return 0;
        }
        */
    }
}
