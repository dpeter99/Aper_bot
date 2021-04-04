using System.Linq;
using System.Threading.Tasks;
using Aper_bot.Database;
using Aper_bot.Database.Model;
using Aper_bot.Events;
using Aper_bot.Modules.CommandProcessing;
using Aper_bot.Modules.CommandProcessing.Attributes;
using Aper_bot.Modules.Discord;
using Aper_bot.Util;
using Brigadier.NET;
using Brigadier.NET.Builder;
using Brigadier.NET.Context;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aper_bot.Modules.Commands.Rules
{
    [CommandProvider]
    class RuleCommands : ChatCommands
    {
        //IDbContextFactory<DatabaseContext> dbFactory;

        public RuleCommands(IDbContextFactory<CoreDatabaseContext> fac)
        {
            //dbFactory = fac;
        }

        public override LiteralArgumentBuilder<CommandExecutionContext> Register(IArgumentContext<CommandExecutionContext> l)
        {
            return l.Literal("rule")
                .Then(l => l.Argument("id", Arguments.Integer(min: 0))
                    .Executes(SingleRule)
                )
                .Then(r => r.Literal("add")
                    .Then(a => a.Argument("text", Arguments.GreedyString())
                        .Executes(AddRule)
                    )
                    .Then(s => s.Literal("at")
                        .Then(n => n.Argument("num", Arguments.Integer())
                            .Then(b => b.Argument("text", Arguments.GreedyString())
                                .Executes(AddRule)
                            )
                        )
                    )
                )
                .Then(r => r.Literal("remove")
                    .Then(i => i.Argument("num", Arguments.Integer())
                        .Executes(RemoveRule)
                    )
                )
                .Then(l => l.Literal("list")
                    .Executes(ListRules)
                )
                .Then(l => l.Literal("setup")
                    .Then(p => p.Literal("place")
                        .Executes(PlaceRules)
                    )
                );
        }

        private async Task RemoveRule(CommandContext<CommandExecutionContext> context, IMessageCreatedEvent discordMessageEvent)
        {
            var guild = discordMessageEvent.Guild;
            if (guild == null)
            {
                discordMessageEvent.RespondError("The server is not set up");
                return;
            }

            var num = context.GetArgument<int>("num");

            await context.Source.Db.Entry(guild).Collection(g => g!.Rules).LoadAsync();

            guild!.Rules.Sort((a, b) => a.Number - b.Number);
            var rule = guild!.Rules.Find(r => r.Number == num);
            if (rule == null)
            {
                discordMessageEvent.RespondError($"Could not find rule {num}");
                return;
            }

            guild.Rules.Remove(rule);
            context.Source.Db.Remove(rule);

            guild.Rules.ContinuouslyNumber(((guildRule, i) => guildRule.Number = i), 1);

            discordMessageEvent.Respond("Removed");
            await context.Source.Db.SaveChangesAsync();

            await UpdateRules(discordMessageEvent);
            return;
        }

        async Task SingleRule(CommandContext<CommandExecutionContext> context, IMessageCreatedEvent discordMessageEvent)
        {
            var guild = discordMessageEvent.Guild;
            if (guild == null)
            {
                discordMessageEvent.RespondError("The server is not set up");
                return;
            }

            var num = context.GetArgument<int>("id");

            await context.Source.Db.Entry(guild).Collection(g => g!.Rules).LoadAsync();

            guild!.Rules.Sort((a, b) => a.Number - b.Number);
            var rule = guild!.Rules.Find(r => r.Number == num);
            if (rule == null)
            {
                discordMessageEvent.RespondError($"Could not find rule {num}");
                return;
            }

            var text =
                $"{DiscordEmoji.FromName(DiscordBot.Instance.Client, $":{rule.Number.ToName()}:")} {rule.Description}";

            var embed = DiscordBot.Instance.BaseEmbed();
            embed.Description = text;

            await discordMessageEvent.Respond(embed.Build());
        }

        async Task PlaceRules(CommandContext<CommandExecutionContext> context, IMessageCreatedEvent discordMessageEvent)
        {
            var guild = discordMessageEvent.Guild;
            if (discordMessageEvent.Guild == null)
            {
                GuildNotSetUp(discordMessageEvent);
            }

            context.Source.Db.Attach(guild);
            await context.Source.Db.Entry(guild).Collection(g => g!.Rules).LoadAsync();

            var text = RulesText(guild);

            var embed = DiscordBot.Instance.BaseEmbed();
            embed.Title = "Server Rules";
            embed.Description = text;
            embed.Author = null;

            var message = await discordMessageEvent.Respond(embed.Build());
            guild.RulesMessageId = message.Id.ToString();
            guild.RulesChannelId = message.ChannelId.ToString();
            await context.Source.Db.SaveChangesAsync();
        }

        [CommandPermissionRequired(PermissionLevels.Admin)]
        async Task AddRule(CommandContext<CommandExecutionContext> context, IMessageCreatedEvent discordMessageEvent)
        {
            var guild = discordMessageEvent.Guild;
            if (guild == null)
            {
                GuildNotSetUp(discordMessageEvent);
                return;
            }

            context.Source.Db.Attach(guild);
            await context.Source.Db.Entry(guild).Collection(g => g!.Rules).LoadAsync();

            guild.Rules.Sort((a, b) => a.Number - b.Number);

            int num = discordMessageEvent.Guild!.Rules.Count + 1;

            if (context.HasArgument<int>("num"))
            {
                num = context.GetArgument<int>("num");
            }

            if (guild.Rules.Exists(r => r.Number == num))
            {
                guild.Rules.Where(i => i.Number >= num).SetValue(r => r.Number++);
                //remove gaps
                //guild.Rules.ContinuouslyNumber(((guildRule, i) => guildRule.Number = i), 1);
            }

            string text = Arguments.GetString(context, "text");
            discordMessageEvent.Guild.Rules.Add(new Database.Model.GuildRule(num, text));

            context.Source.Db.Update(discordMessageEvent.Guild);
            await context.Source.Db.SaveChangesAsync();

            //TODO: Nicer embed response
            await discordMessageEvent.Respond("Added");

            await UpdateRules(discordMessageEvent);
        }


        async Task ListRules(CommandContext<CommandExecutionContext> context, IMessageCreatedEvent discordMessageEvent)
        {
            var guild = discordMessageEvent.Guild;
            if (guild == null)
            {
                GuildNotSetUp(discordMessageEvent);
                return;
            }


            context.Source.Db.Attach(guild);
            await context.Source.Db.Entry(guild).Collection(g => g!.Rules).LoadAsync();


            var text = RulesText(guild);

            var embed = new DiscordEmbedBuilder()
            {
                Description = text
            };

            await discordMessageEvent.Respond(embed.Build());
        }

        private static string RulesText(Guild guild)
        {
            var text = "";
            guild!.Rules.Sort((a, b) => a.Number - b.Number);
            foreach (var item in guild!.Rules)
            {
                text +=
                    $"\n\n{DiscordEmoji.FromName(DiscordBot.Instance.Client, $":{item.Number.ToName()}:")} {item.Description}";
            }

            return text;
        }

        private async Task UpdateRules(IMessageCreatedEvent messageEvent)
        {
            var guild = messageEvent.Guild;
            if (guild?.RulesChannelId == null ||
                guild?.RulesMessageId == null)
            {
                return;
            }

            var text = RulesText(guild);

            var embed = DiscordBot.Instance.BaseEmbed();
            embed.Title = "Server Rules";
            embed.Description = text;
            embed.Author = null;
            if (guild.RulesChannelId != null && guild.RulesMessageId != null)
            {
                var ch = ((DiscordMessageCreatedEvent)messageEvent).@event.Guild.GetChannel(guild.RulesChannelId ?? 0L);
                var me = await ch.GetMessageAsync(guild.RulesMessageId ?? 0L);
                await me.ModifyAsync(embed: embed.Build());
            }
        }


        private static void GuildNotSetUp(IMessageCreatedEvent source)
        {
            source.RespondError("The server is not set up");
        }
    }
}