﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aper_bot.Database.Model;
using Aper_bot.Events;
using Aper_bot.Modules.CommandProcessing;
using Aper_bot.Modules.CommandProcessing.Attributes;
using Aper_bot.Modules.CommandProcessing.DiscordArguments;
using Aper_bot.Modules.Discord;
using Aper_bot.Modules.Discord.Config;
using DSharpPlus;
using DSharpPlus.Entities;
using Mars;
using Microsoft.Extensions.Options;
using Serilog;

namespace Aper_bot.Modules.Commands
{
    [CommandProvider]
    class ServerSettingsCommand : ChatCommands
    {
        //IDbContextFactory<DatabaseContext> dbContextFactory;
        ILogger logger;
        IOptions<DiscordConfig> config;

        public ServerSettingsCommand(ILogger log, IOptions<DiscordConfig> options)
        {
            //dbContextFactory = db;
            logger = log;
            config = options;
        }

        public override IEnumerable<CommandNode> Register()
        {
            var quote = new LiteralNode("server",new CommandMetaData(1));

            quote.NextLiteral("setup")
                .ThisCalls(ServerSetup);
            
            /*
            return l.Literal("server")
                .Then(s =>
                    s.Literal("setup")
                            .Executes(AsyncExecue(ServerSetup))
                )
                .Then(p=>
                    p.Literal("role")
                        .Then(a=> a.Literal("add")
                            .Then( a=> a.Argument("level",Arguments.String())
                                .Then(b=> b.Argument("role",DiscordArgumentTypes.Role())
                                        .Executes(SetRole)
                                )
                            )
                        )
                        .Then(a=> a.Literal("remove")
                            .Then( a=> a.Argument("level",Arguments.String())
                                .Then(b=> b.Argument("role",DiscordArgumentTypes.Role())
                                        .Executes(RemoveRole)
                                )
                            )
                        )
                );
                */
            return null;
        }

        
        
        [CommandPermissionRequired(PermissionLevels.Owner)]
        private async Task ServerSetup(ParseResult result, IMessageCreatedEvent context)
        {
            
            if (context.Guild != null)
            {
                await context.Respond("Server already active");
                return;
            }

            if (context is DiscordMessageCreatedEvent dmce)
            {
                var message = dmce.@event;

                DiscordGuild discord_guild = message.Guild;


                var member = await message.Guild.GetMemberAsync(message.Message.Author.Id);

                var permissions = member.PermissionsIn(message.Message.Channel);
                if (permissions.HasPermission(Permissions.Administrator) || member.Id.ToString() == config.Value.Owner)
                {
                    
                    Guild? guild = (from g in context.Db.Guilds
                        where g.GuildID == discord_guild.Id
                        select g).FirstOrDefault();

                    if (guild == null)
                    {

                        logger.Information($"Registering guild: {discord_guild.Id}");

                        context.Db.Add(new Guild(discord_guild.Name, discord_guild.Id.ToString()));
                        context.Db.SaveChanges();

                        var embed = new DSharpPlus.Entities.DiscordEmbedBuilder()
                        {
                            Description = "Server setup complete!"
                        };

                        await message.Message.RespondAsync(embed: embed.Build());
                    }


                }
                else
                {
                    await message.Message.RespondAsync("You do not have the permission");
                }

            }
        }
        
        
        /*
        [CommandPermissionRequired(PermissionLevels.Admin)]
        private async Task SetRole(CommandContext<CommandExecutionContext> ctx, IMessageCreatedEvent discordMessageEvent)
        {
            if (discordMessageEvent.Guild == null)
            {
                await discordMessageEvent.RespondError("This command can only be run in a server");
            }
            
            var levelName = Arguments.GetString(ctx,"level");
            if (Enum.TryParse(levelName, out PermissionLevels level))
            {
                
                await ctx.Source.Db.Entry(discordMessageEvent.Guild)
                    .Collection(g => g!.PermissionLevels).LoadAsync();
                
                var role = ctx.GetArgument<DiscordRole>("role");

                var setting = (from p in discordMessageEvent.Guild!.PermissionLevels
                    where p.PermissionLevel == level && p.RoleID == role.Id.ToString()
                        select p).FirstOrDefault();
                if (setting == null)
                {
                    discordMessageEvent.Guild!.PermissionLevels.Add(new GuildPermissionLevel(role.Id.ToString(),level));    
                    await discordMessageEvent.Respond("Done");
                }
                else
                {
                    await discordMessageEvent.Respond("Already added");
                }
               

                await ctx.Source.Db.SaveChangesAsync();
            }
        }
        
        private async Task RemoveRole(CommandContext<CommandExecutionContext> ctx, IMessageCreatedEvent discordMessageEvent)
        {
            if (discordMessageEvent.Guild == null)
            {
                await discordMessageEvent.RespondError("This command can only be run in a server");
            }
            
            var levelName = Arguments.GetString(ctx,"level");
            var role = ctx.GetArgument<DiscordRole>("role");
            if (Enum.TryParse(levelName, out PermissionLevels level))
            {
                
                await ctx.Source.Db.Entry(discordMessageEvent.Guild)
                    .Collection(g => g!.PermissionLevels).LoadAsync();
                
                var setting = (from p in discordMessageEvent.Guild!.PermissionLevels
                    where p.PermissionLevel == level && p.RoleID == role.Id.ToString()
                    select p).FirstOrDefault();
                
                if (setting != null)
                {
                    discordMessageEvent.Guild!.PermissionLevels.Remove(setting);    
                    await discordMessageEvent.Respond("Done");
                }
                else
                {
                    await discordMessageEvent.Respond("Didn't find");
                }
               

                await ctx.Source.Db.SaveChangesAsync();
            }
        }

        */
    }
}
