using Aper_bot.Database;
using Aper_bot.Database.Model;
using Aper_bot.Modules.Commands;
using Aper_bot.Modules.Commands.DiscordArguments;

using Brigadier.NET;
using Brigadier.NET.Builder;
using Brigadier.NET.Context;

using DSharpPlus.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Aper_bot.Modules
{

    /// <summary>
    /// 
    /// 
    /// </summary>
    /// <example>
    /// /quotes add from @dpeter99 This is a cool thing
    /// /quotes add This is a cool thing
    /// 
    /// </example>

    [CommandProvider]
    class QuotesCommand : ChatCommands
    {
        Serilog.ILogger logger;

        public QuotesCommand(Serilog.ILogger log)
        {
            logger = log;
        }

        public override LiteralArgumentBuilder<CommandSourceStack> Register(IArgumentContext<CommandSourceStack> l)
        {
            return l.Literal("/quote")
                .Then(q =>
                    q.Literal("add")
                        .Then(a =>
                            a.Argument("text", Arguments.GreedyString())
                                .Executes(AsyncExecue(AddQuote))
                        )
                        .Then(s =>
                              s.Literal("from").Then(a =>
                                  a.Argument("source", DiscordArgumentTypes.User())
                                    .Then(t =>
                                        t.Argument("text", Arguments.GreedyString())
                                            .Executes(AsyncExecue(AddQuote))
                                      )
                              )
                        )
                )
                .Then(l =>
                    l.Literal("list")
                        .Then(u =>
                            u.Argument("user", DiscordArgumentTypes.User())
                                .Executes(AsyncExecue(ListQuotes))
                        )
                        .Executes(AsyncExecue(ListQuotes))
                )
                .Then(s =>
                    s.Argument("number", Arguments.Integer())
                        .Executes(AsyncExecue(PrintQuote))
                )
                ;
        }

        private async Task PrintQuote(CommandContext<CommandSourceStack> ctx)
        {
            var db = ctx.Source.db;

            var num = Arguments.GetInteger(ctx, "number");

            var guild = ctx.Source.guild;
            if (guild != null)
            {
                var quote = (from q in db.Quotes
                             where q.number == num && q.GuildID == guild.ID
                             select q).FirstOrDefault();
                if (quote != null)
                {
                    var embed = new DiscordEmbedBuilder()
                    {
                        Description = quote.Text
                    };

                    await ctx.Source.@event.Message.RespondAsync(embed: embed.Build());
                    return;
                }

            }
        }

        private async Task ListQuotes(CommandContext<CommandSourceStack> ctx)
        {
            var db = ctx.Source.db;

            var guild = ctx.Source.guild;
            if (guild != null)
            {
                await db.Entry(guild).Collection(g => g!.Quotes).LoadAsync();
                string text = "";
                foreach (var item in guild.Quotes)
                {
                    text += $"\n{item.Text}";
                }

                var embed = new DiscordEmbedBuilder()
                {
                    Description = text
                };

                await ctx.Source.@event.Message.RespondAsync(embed: embed.Build());
            }


        }

        private async Task AddQuote(CommandContext<CommandSourceStack> ctx)
        {
            var db = ctx.Source.db;
            if (ctx.Source.guild == null)
            {
                return;
            }

            User creator = ctx.Source.author;
            User? source = null;

            DateTime date = ctx.Source.@event.Message.Timestamp.DateTime;

            string text = Arguments.GetString(ctx, "text");


            if (ctx.HasArgument<string>("source"))
            {
                var user = DiscordArgumentTypes.GetUser(ctx, "source");
                source = db.GetOrCreateUserFor(user);
                //db.SaveChanges();
            }

            db.Add(new Quote(creator.ID, source?.ID, ctx.Source.guild.ID, date, DateTime.MinValue, text, null));

            db.SaveChanges();


        }
    }
}
