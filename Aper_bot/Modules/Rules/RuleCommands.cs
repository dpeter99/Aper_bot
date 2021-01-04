using Aper_bot.Database;
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
        IDbContextFactory<DatabaseContext> dbFactory;

        public RuleCommands(IDbContextFactory<DatabaseContext> fac)
        {
            dbFactory = fac;
        }

        public override LiteralArgumentBuilder<CommandSourceStack> Register(IArgumentContext<CommandSourceStack> l)
        {
            return l.Literal("/rule")
                    .Then(
                        r =>
                        r.Literal("add").Then(
                            a =>
                            {
                                return a.Argument("text", Arguments.GreedyString())
                                        .Executes(AsyncExecue( AddRule));
                            } 
                            )
                        
                    )
                    .Then(
                        l=>
                        l.Literal("list").Executes(AsyncExecue(ListRules))
                       );
        }

        async Task AddRule(CommandContext<CommandSourceStack> context)
        {
            var source = context.Source;
            var guild = context.Source.guild;
            if (source.guild == null)
            {
                GuildNotSetUp(source);

            }

            using (var db = dbFactory.CreateDbContext())
            {
                db.Attach(guild);
                await db.Entry(guild).Collection(g => g!.Rules).LoadAsync();

                int n = source.guild!.Rules.Count+1;
                string text = Arguments.GetString(context, "text");
                source.guild.Rules.Add(new Database.Model.GuildRule(n, text));

                db.Update(source.guild);
                await db.SaveChangesAsync();

                await source.@event.Message.RespondAsync("Added");
            }
        }



        async Task ListRules(CommandContext<CommandSourceStack> context)
        {
            var guild = context.Source.guild;
            if (guild == null)
            {
                GuildNotSetUp(context.Source);
            }

            using (var db = dbFactory.CreateDbContext())
            {
                db.Attach(guild);
                await db.Entry(guild).Collection(g => g!.Rules).LoadAsync();
            }



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

            await context.Source.@event.Message.RespondAsync(embed: embed.Build());
        }

        private static void GuildNotSetUp(CommandSourceStack source)
        {
            source.@event.Message.RespondAsync("The server is not set up");
        }
    }
}
