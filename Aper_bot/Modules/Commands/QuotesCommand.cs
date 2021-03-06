﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aper_bot.Database.Model;
using Aper_bot.Events;
using Aper_bot.Modules.CommandProcessing;
using Aper_bot.Modules.CommandProcessing.Attributes;
using Aper_bot.Modules.CommandProcessing.DiscordArguments;
using Aper_bot.Modules.Discord;
using Aper_bot.Util.Discord;
using DSharpPlus.Entities;
using Mars;
using Mars.Arguments;
using Microsoft.EntityFrameworkCore;

namespace Aper_bot.Modules.Commands
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

        public override IEnumerable<CommandNode> Register()
        {
            var quote = new LiteralNode("quote", new CommandMetaData(1));

            //#### ADD ######
            var add = quote.NextLiteral("add");
            add.NextLiteral("anonymous")
                .NextArgument("text", new LongStringArgument())
                .ThisCalls(new CommandFunction(AddQuote));

            add.NextLiteral("from")
                .NextArgument("source", DiscordArgumentTypes.User())
                .NextArgument("text", new LongStringArgument())
                .ThisCalls(new CommandFunction(AddQuote));

            add.NextLiteral("repl")
                .ThisCalls(AddQuote);

            //#### LIST ######
            var list = quote.NextLiteral("get");

            list.NextLiteral("all").ThisCalls(new CommandFunction(ListQuotes));


            //#### Specific number ######
            list.NextLiteral("num").NextArgument("number", new IntArgument()).ThisCalls(new CommandFunction(PrintQuote));

            //#### REMOVE ######
            var remove = quote.NextLiteral("remove");
            remove.NextArgument("id", new IntArgument()).ThisCalls(new CommandFunction(RemoveQuote));


            return new[] {quote};
        }


        [GuildRequiered]
        private async Task RemoveQuote(ParseResult result, IMessageCreatedEvent context)
        {
            var db = context.Db;

            var num = result.GetIntArg("id");

            var guild = context.Guild;

            var quote = (from q in db.Quotes
                where q.number == num && q.GuildID == guild!.ID
                select q).FirstOrDefault();

            if (quote != null)
            {
                await db.Entry(quote).Reference(r => r.Source).LoadAsync();

                guild!.Quotes.Remove(quote);
                await db.SaveChangesAsync();

                var embed = DiscordBot.Instance.BaseEmbed();
                embed.Title = $"{EmojiHelper.Cross()} Removed Quote";
                embed.Color = new Optional<DiscordColor>(new DiscordColor(255, 0, 0));
                embed.Description = $"> {quote.Text}\nby {quote.SourceName}";
                embed.Timestamp = quote.CreationTime.Year > 1 ? quote.CreationTime : new DateTimeOffset();


                embed.Footer = new DiscordEmbedBuilder.EmbedFooter()
                {
                    Text = $"on {quote.CreationTime.ToLongDateString()}"
                };


                await context.Respond(embed.Build());
                return;
            }
            else
            {
                throw new Exception($"There was no quote {num}");
            }
        }

        [GuildRequiered]
        private async Task PrintQuote(ParseResult result, IMessageCreatedEvent context)
        {
            var db = context.Db;

            var num = result.GetIntArg("number");

            var guild = context.Guild;
            if (guild != null)
            {
                var quote = (from q in db.Quotes.Include(a => a.Source)
                    where q.number == num && q.GuildID == guild.ID
                    select q).FirstOrDefault();
                if (quote != null)
                {
                    var embed = DiscordBot.Instance.BaseEmbed();
                    embed.Description = quote.Text;

                    if (quote.CreationTime.Year <= 1)
                    {
                        embed.Timestamp = new DateTimeOffset();
                    }
                    else
                    {
                        embed.Timestamp = quote.EventTime;
                    }

                    embed.Author = new DiscordEmbedBuilder.EmbedAuthor();
                    if (quote.Source != null)
                    {
                        var discordGuild = await DiscordBot.Instance.Client.GetGuildAsync(guild.GuildID);
                        var sourceMember = await discordGuild.GetMemberAsync(ulong.Parse(quote.Source.UserID));

                        embed.Author.IconUrl = sourceMember.AvatarUrl;
                        embed.Author.Name = sourceMember.Nickname ?? sourceMember.Username;
                    }

                    embed.Footer = new DiscordEmbedBuilder.EmbedFooter()
                    {
                        Text = $"on {quote.CreationTime.ToLongDateString()}"
                    };


                    await context.Respond(embed.Build());
                }
            }
        }


        [GuildRequiered]
        private async Task ListQuotes(ParseResult result, IMessageCreatedEvent context)
        {
            var db = context.Db;

            var guild = context.Guild;
            if (guild != null)
            {
                var discordGuildAsync = DiscordBot.Instance.Client.GetGuildAsync(guild.GuildID);

                string text = "";

                var q = from h in db.Quotes.Include(blog => blog.Source)
                    where h.Guild == guild
                    select h;

                foreach (var quote in q)
                {
                    if (quote.Source != null)
                    {
                        var discordGuild = await discordGuildAsync;
                        var sourceMember = (await discordGuild.GetMemberAsync(ulong.Parse(quote.Source.UserID))).Nickname;
                        //var sourceMember = quote.Source.Name;

                        text += $"\n{EmojiHelper.Number(quote.number)} *{quote.Text}* - by {sourceMember}";
                    }
                    else
                    {
                        text += $"\n{EmojiHelper.Number(quote.number)} *{quote.Text}* - by *Anonymous*";
                    }
                }

                var embed = DiscordBot.Instance.BaseEmbed();
                embed.Title = "Qotes";
                embed.Description = text;
                embed.Author = null;


                await context.Respond(embed.Build());
            }
        }


        [GuildRequiered]
        private async Task AddQuote(ParseResult result, IMessageCreatedEvent context)
        {
            var db = context.Db;

            User creator = context.Author;
            User? source = null;

            DateTime date = context.Time;
            DateTime eventTime = context.Time;

            string text = "";

            if (context.HasRefMessage)
            {
                text = context.RefMessageText!;
                source = context.RefMessageAuthor ?? null;
                eventTime = context.RefMessageTime?.Date ?? context.Time;
            }
            else
            {
                if (result.Args.ContainsKey("text"))
                {
                    text = result.GetStringArg("text");
                }

                if (string.IsNullOrWhiteSpace(text))
                {
                    await context.RespondError("Dind't find text to quote");
                    return;
                }


                if (result.Args.ContainsKey("source"))
                {
                    var user = (DiscordUser) result.Args["source"];
                    source = db.GetOrCreateUserFor(user);
                }
            }


            await db.Entry(context.Guild).Collection(d => d!.Quotes).LoadAsync();

            context.Guild!.Quotes.Sort((a, b) => a.number - b.number);
            var last = context.Guild.Quotes.LastOrDefault();
            var next_num = last?.number + 1 ?? 0;

            var entity = new Quote(creator.ID, source?.ID, context.Guild.ID, date, eventTime, text, null, next_num);
            db.Add(entity);

            var embed = DiscordBot.Instance.BaseEmbed();
            embed.Color = new Optional<DiscordColor>(new DiscordColor(0, 255, 0));
            embed.Title = $"Added new quote number {entity.number}:";
            embed.Description = $"> {text}";
            embed.Footer = new DiscordEmbedBuilder.EmbedFooter()
            {
                Text = $"by {entity.SourceName}"
            };

            await context.Respond(embed.Build());

            await db.SaveChangesAsync();
        }
    }
}