using System;
using System.Linq;
using System.Threading.Tasks;
using Aper_bot.Database.Model;
using Aper_bot.Events;
using Aper_bot.Modules.CommandProcessing;
using Aper_bot.Modules.CommandProcessing.Attributes;
using Aper_bot.Modules.CommandProcessing.DiscordArguments;
using Aper_bot.Modules.Discord;
using Aper_bot.Util;
using Brigadier.NET;
using Brigadier.NET.Builder;
using Brigadier.NET.Context;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aper_bot.Command
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

        public override LiteralArgumentBuilder<CommandArguments> Register(IArgumentContext<CommandArguments> l)
        {
            return l.Literal("/quote")
                    .Then(q =>
                        q.Literal("add")
                            .Then(an=>
                                an.Literal("anonym").Then(a =>
                                    a.Argument("text", Arguments.GreedyString())
                                        .Executes(AddQuote)
                                )
                            )
                            .Then(s =>
                                s.Literal("from").Then(a =>
                                    a.Argument("source", DiscordArgumentTypes.User())
                                        .Then(t =>
                                            t.Argument("text", Arguments.GreedyString())
                                                .Executes(AddQuote)
                                        )
                                )
                            )
                    )
                    .Then(l =>
                        l.Literal("list")
                            .Then(u =>
                                u.Argument("user", DiscordArgumentTypes.User())
                                    .Executes(ListQuotes)
                            )
                            .Executes(ListQuotes)
                    )
                    .Then(l =>
                        l.Literal("remove")
                            .Then(u =>
                                u.Argument("id", Arguments.Integer())
                                    .Executes(RemoveQuote)
                            )
                    )
                    .Then(s =>
                        s.Argument("number", Arguments.Integer())
                            .Executes(PrintQuote)
                    )
                ;
        }

        [GuildRequiered(true)]
        private async Task RemoveQuote(CommandContext<CommandArguments> ctx, MessageCreatedEvent messageEvent)
        {
            var db = messageEvent.db;

            var num = Arguments.GetInteger(ctx, "id");

            var guild = messageEvent.guild;

            var quote = (from q in db.Quotes
                where q.number == num && q.GuildID == guild!.ID
                select q).FirstOrDefault();
            if (quote != null)
            {
                guild!.Quotes.Remove(quote);
                await db.SaveChangesAsync();

                var embed = DiscordBot.Instance.BaseEmbed();
                embed.Title = $"{EmojiHelper.Cross()} Removed Quote";
                embed.Color = new Optional<DiscordColor>(new DiscordColor(255, 0, 0));
                embed.Description = $"> {quote.Text}\nby {quote.SourceName}";
                embed.Timestamp = quote.CreationTime;


                embed.Footer = new DiscordEmbedBuilder.EmbedFooter()
                {
                    Text = $"on {quote.CreationTime.ToLongDateString()}"
                };


                messageEvent.Respond(embed.Build());
                return;
            }
            else
            {
                throw new Exception($"There was no quote {num}");
            }
        }

        private async Task PrintQuote(CommandContext<CommandArguments> ctx, MessageCreatedEvent messageEvent)
        {
            var db = messageEvent.db;

            var num = Arguments.GetInteger(ctx, "number");

            var guild = messageEvent.guild;
            if (guild != null)
            {
                var quote = (from q in db.Quotes.Include(a => a.Source)
                    where q.number == num && q.GuildID == guild.ID
                    select q).FirstOrDefault();
                if (quote != null)
                {
                    var embed = DiscordBot.Instance.BaseEmbed();
                    embed.Description = quote.Text;
                    embed.Timestamp = quote.CreationTime;
                    embed.Author = new DiscordEmbedBuilder.EmbedAuthor();
                    if (quote.Source != null)
                    {
                        var source_user =
                            await DiscordBot.Instance.Client.GetUserAsync(ulong.Parse(quote.Source.UserID));
                        embed.Author.IconUrl = source_user.AvatarUrl;
                        embed.Author.Name = quote.SourceName;
                    }

                    embed.Footer = new DiscordEmbedBuilder.EmbedFooter()
                    {
                        Text = $"on {quote.CreationTime.ToLongDateString()}"
                    };


                    messageEvent.Respond(embed.Build());
                    return;
                }
            }
        }

        private async Task ListQuotes(CommandContext<CommandArguments> ctx, MessageCreatedEvent messageEvent)
        {
            var db = messageEvent.db;

            var guild = messageEvent.guild;
            if (guild != null)
            {
                await db.Entry(guild).Collection(g => g!.Quotes).LoadAsync();
                string text = "";
                foreach (var item in guild.Quotes)
                {
                    text += $"\n{EmojiHelper.Number(item.number)} *{item.Text}* - by {item.SourceName}";
                }

                var embed = DiscordBot.Instance.BaseEmbed();
                embed.Title = "Qotes";
                embed.Description = text;
                embed.Author = null;


                await messageEvent.@event.Message.RespondAsync(embed: embed.Build());
            }
        }

        [GuildRequiered(true)]
        private async Task AddQuote(CommandContext<CommandArguments> ctx, MessageCreatedEvent messageEvent)
        {
            var db = messageEvent.db;

            User creator = messageEvent.author;
            User? source = null;

            DateTime date = messageEvent.@event.Message.Timestamp.DateTime;

            string text = Arguments.GetString(ctx, "text");


            if (ctx.HasArgument<string>("source"))
            {
                var user = DiscordArgumentTypes.GetUser(ctx, "source");
                source = db.GetOrCreateUserFor(user);
            }

            await db.Entry(messageEvent.guild).Collection(d => d!.Quotes).LoadAsync();

            messageEvent.guild!.Quotes.Sort((a, b) => a.number - b.number);
            var last = messageEvent.guild.Quotes.LastOrDefault();
            var next_num = last?.number + 1 ?? 0;

            var entity = new Quote(creator.ID, source?.ID, messageEvent.guild.ID, date, DateTime.MinValue, text, null,
                next_num);
            db.Add(entity);

            var embed = DiscordBot.Instance.BaseEmbed();
            embed.Color = new Optional<DiscordColor>(new DiscordColor(0, 255, 0));
            embed.Title = $"Added new quote number {entity.number}:";
            embed.Description = $"> {text}";
            embed.Footer = new DiscordEmbedBuilder.EmbedFooter()
            {
                Text = $"by {entity.SourceName}"
            };

            await messageEvent.Respond(embed.Build());

            await db.SaveChangesAsync();
        }
    }
}