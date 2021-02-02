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

namespace Aper_bot.Command.Rules
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

        private async Task RemoveRule(CommandContext<CommandArguments> context, MessageCreatedEvent messageEvent)
        {
            var guild = messageEvent.guild;
            if (guild == null)
            {
                messageEvent.RespondError("The server is not set up");
                return;
            }

            var num = context.GetArgument<int>("num");

            await messageEvent.db.Entry(guild).Collection(g => g!.Rules).LoadAsync();

            guild!.Rules.Sort((a, b) => a.Number - b.Number);
            var rule = guild!.Rules.Find(r => r.Number == num);
            if (rule == null)
            {
                messageEvent.RespondError($"Could not find rule {num}");
                return;
            }

            guild.Rules.Remove(rule);
            messageEvent.db.Remove(rule);

            guild.Rules.ContinuouslyNumber(((guildRule, i) => guildRule.Number = i), 1);

            messageEvent.Respond("Removed");
            await messageEvent.db.SaveChangesAsync();

            await UpdateRules(messageEvent);
            return;
        }

        async Task SingleRule(CommandContext<CommandArguments> context, MessageCreatedEvent messageEvent)
        {
            var guild = messageEvent.guild;
            if (guild == null)
            {
                messageEvent.RespondError("The server is not set up");
                return;
            }

            var num = context.GetArgument<int>("id");

            await messageEvent.db.Entry(guild).Collection(g => g!.Rules).LoadAsync();

            guild!.Rules.Sort((a, b) => a.Number - b.Number);
            var rule = guild!.Rules.Find(r => r.Number == num);
            if (rule == null)
            {
                messageEvent.RespondError($"Could not find rule {num}");
                return;
            }

            var text =
                $"{DiscordEmoji.FromName(DiscordBot.Instance.Client, $":{rule.Number.ToName()}:")} {rule.Description}";

            var embed = DiscordBot.Instance.BaseEmbed();
            embed.Description = text;

            await messageEvent.@event.Message.RespondAsync(embed: embed.Build());
        }

        async Task PlaceRules(CommandContext<CommandArguments> context, MessageCreatedEvent messageEvent)
        {
            var guild = messageEvent.guild;
            if (messageEvent.guild == null)
            {
                GuildNotSetUp(messageEvent);
            }

            messageEvent.db.Attach(guild);
            await messageEvent.db.Entry(guild).Collection(g => g!.Rules).LoadAsync();

            var text = RulesText(guild);

            var embed = DiscordBot.Instance.BaseEmbed();
            embed.Title = "Server Rules";
            embed.Description = text;
            embed.Author = null;

            var message = await messageEvent.Respond(embed.Build());
            guild.RulesMessageId = message.Id.ToString();
            guild.RulesChannelId = message.ChannelId.ToString();
            await messageEvent.db.SaveChangesAsync();
        }

        [CommandPermissionRequired(PermissionLevels.Admin)]
        async Task AddRule(CommandContext<CommandArguments> context, MessageCreatedEvent messageEvent)
        {
            var guild = messageEvent.guild;
            if (guild == null)
            {
                GuildNotSetUp(messageEvent);
                return;
            }

            messageEvent.db.Attach(guild);
            await messageEvent.db.Entry(guild).Collection(g => g!.Rules).LoadAsync();

            guild.Rules.Sort((a, b) => a.Number - b.Number);

            int num = messageEvent.guild!.Rules.Count + 1;

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
            messageEvent.guild.Rules.Add(new Database.Model.GuildRule(num, text));

            messageEvent.db.Update(messageEvent.guild);
            await messageEvent.db.SaveChangesAsync();

            //TODO: Nicer embed response
            await messageEvent.@event.Message.RespondAsync("Added");

            await UpdateRules(messageEvent);
        }


        async Task ListRules(CommandContext<CommandArguments> context, MessageCreatedEvent messageEvent)
        {
            var guild = messageEvent.guild;
            if (guild == null)
            {
                GuildNotSetUp(messageEvent);
                return;
            }


            messageEvent.db.Attach(guild);
            await messageEvent.db.Entry(guild).Collection(g => g!.Rules).LoadAsync();


            var text = RulesText(guild);

            var embed = new DiscordEmbedBuilder()
            {
                Description = text
            };

            await messageEvent.@event.Message.RespondAsync(embed: embed.Build());
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

        private async Task UpdateRules(MessageCreatedEvent messageEvent)
        {
            var guild = messageEvent.guild;
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
                var ch = messageEvent.@event.Guild.GetChannel(guild.RulesChannelId ?? 0L);
                var me = await ch.GetMessageAsync(guild.RulesMessageId ?? 0L);
                await me.ModifyAsync(embed: embed.Build());
            }
        }


        private static void GuildNotSetUp(MessageCreatedEvent source)
        {
            source.RespondError("The server is not set up");
        }
    }
}