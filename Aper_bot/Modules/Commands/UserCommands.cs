using Aper_bot.Database;
using Aper_bot.Database.Model;
using Aper_bot.Modules.Commands.DiscordArguments;

using Brigadier.NET.Builder;
using Brigadier.NET.Context;

using Microsoft.EntityFrameworkCore;

using Serilog;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Aper_bot.Modules.Commands
{
    [CommandProvider]
    class UserCommands : ChatCommands
    {
        Serilog.ILogger logger;

        IDbContextFactory<DatabaseContext> dbContextFactory;

        public UserCommands(Serilog.ILogger log, IDbContextFactory<DatabaseContext> fac)
        {
            logger = log;
            dbContextFactory = fac;
        }

        public override LiteralArgumentBuilder<CommandSourceStack> Register(IArgumentContext<CommandSourceStack> l)
        {
            return l.Literal("/user")
                    .Then(
                        a =>
                        a.Literal("info")
                            .Then(
                                i =>
                                    i.Argument("user", DiscordArgumentTypes.User())
                                     .Executes(User)
                                )


                    )
                    .Then(
                        a =>
                        a.Argument("user", DiscordArgumentTypes.User())
                         .Executes(User)
                    );

        }

        public int User(CommandContext<CommandSourceStack> context)
        {
            using (DatabaseContext db = dbContextFactory.CreateDbContext())
            {
                DSharpPlus.EventArgs.MessageCreateEventArgs @event = context.Source.@event;
                var user = (from u in db.Users
                            where u.Name == @event.Author.Username
                            select u)
                           .FirstOrDefault();

                if (user != null)
                {
                    @event.Message.RespondAsync($"{user.Name} is a nice fellow");
                }
                else
                {
                    db.Add(new User(@event.Author));
                    db.SaveChangesAsync().Wait();
                    @event.Message.RespondAsync($"I have never see this person. Gona remember {@event.Author.Username}.");
                }
            }
            return 0;
        }

        public int UserInfo(CommandContext<CommandSourceStack> context)
        {
            using (DatabaseContext db = dbContextFactory.CreateDbContext())
            {
                DSharpPlus.EventArgs.MessageCreateEventArgs @event = context.Source.@event;
                var user = (from u in db.Users
                            where u.Name == @event.Author.Username
                            select u)
                           .FirstOrDefault();

                if (user != null)
                {
                    @event.Message.RespondAsync($"{user.Name} is a nice fellow");
                }
                else
                {
                    db.Add(new User(@event.Author));
                    db.SaveChangesAsync().Wait();
                    @event.Message.RespondAsync($"I have never see this person. Gona remember {@event.Author.Username}.");
                }
            }
            return 0;
        }
    }
}
