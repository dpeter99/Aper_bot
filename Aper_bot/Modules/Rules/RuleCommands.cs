using Aper_bot.Database;
using Aper_bot.Events;
using Aper_bot.Modules.Commands;
using Aper_bot.Util;

using Brigadier.NET;
using Brigadier.NET.Builder;
using Brigadier.NET.Context;

using DSharpPlus.Entities;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Aper_bot.Modules.Rules
{
    [CommandProvider]
    class RuleCommands : ChatCommands
    {
        //IDbContextFactory<DatabaseContext> dbFactory;

        public RuleCommands(IDbContextFactory<DatabaseContext> fac)
        {
            //dbFactory = fac;
        }

        public override LiteralArgumentBuilder<CommandArguments> Register(IArgumentContext<CommandArguments> l)
        {
            return l.Literal("/rule")
                    .Then(
                        r =>
                        r.Literal("add").Then(
                            a =>
                            {
                                return a.Argument("text", Arguments.GreedyString())
                                        .Executes(AsyncExecue(AddRule));
                            }
                            )

                    )
                    .Then(
                        l =>
                        l.Literal("list").Executes(AsyncExecue(ListRules))
                       );
        }

        async Task AddRule(CommandContext<CommandArguments> context, MessageCreatedEvent messageEvent)
        {

            var guild = messageEvent.guild;
            if (messageEvent.guild == null)
            {
                GuildNotSetUp(messageEvent);

            }


            messageEvent.db.Attach(guild);
            await messageEvent.db.Entry(guild).Collection(g => g!.Rules).LoadAsync();

            int n = messageEvent.guild!.Rules.Count + 1;
            string text = Arguments.GetString(context, "text");
            messageEvent.guild.Rules.Add(new Database.Model.GuildRule(n, text));

            messageEvent.db.Update(messageEvent.guild);
            await messageEvent.db.SaveChangesAsync();

            await messageEvent.@event.Message.RespondAsync("Added");

        }



        async Task ListRules(CommandContext<CommandArguments> context, MessageCreatedEvent messageEvent)
        {
            var guild = messageEvent.guild;
            if (guild == null)
            {
                GuildNotSetUp(messageEvent);
            }


            messageEvent.db.Attach(guild);
            await messageEvent.db.Entry(guild).Collection(g => g!.Rules).LoadAsync();




            var text = "";
            guild!.Rules.Sort((a, b) => a.Number - b.Number);
            foreach (var item in guild!.Rules)
            {
                text += $"\n\n{DiscordEmoji.FromName(DiscordBot.Instance.Client, $":{item.Number.ToName()}:")} {item.Description}";
            }

            var embed = new DiscordEmbedBuilder()
            {
                Description = text
            };

            await messageEvent.@event.Message.RespondAsync(embed: embed.Build());
        }

        private static void GuildNotSetUp(MessageCreatedEvent source)
        {
            source.@event.Message.RespondAsync("The server is not set up");
        }
    }
}
