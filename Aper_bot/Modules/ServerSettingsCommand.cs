using Aper_bot.Database;
using Aper_bot.Database.Model;
using Aper_bot.Events;
using Aper_bot.Modules.Commands;
using Aper_bot.Modules.Commands.DiscordArguments;

using Brigadier.NET;
using Brigadier.NET.Builder;
using Brigadier.NET.Context;

using DSharpPlus;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Serilog;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace Aper_bot.Modules
{
    [CommandProvider]
    class ServerSettingsCommand : ChatCommands
    {
        //IDbContextFactory<DatabaseContext> dbContextFactory;
        ILogger logger;
        IOptions<Config> config;

        public ServerSettingsCommand(ILogger log, IOptions<Config> options)
        {
            //dbContextFactory = db;
            logger = log;
            config = options;
        }

        public override LiteralArgumentBuilder<CommandArguments> Register(IArgumentContext<CommandArguments> l)
        {
            return l.Literal("/server")
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
        }

        [CommandPermissionRequired(PermissionLevels.Owner)]
        private async Task ServerSetup(CommandContext<CommandArguments> ctx, MessageCreatedEvent messageEvent)
        {
            var message = messageEvent.@event;

            DSharpPlus.Entities.DiscordGuild discord_guild = message.Guild;

            if (messageEvent.guild != null)
            {
                await message.Message.RespondAsync("Server already active");
                return;
            }

            var member = await message.Guild.GetMemberAsync(message.Message.Author.Id);

            var permissions = member.PermissionsIn(message.Message.Channel);
            if (permissions.HasPermission(Permissions.Administrator) || member.Id.ToString() == config.Value.Owner)
            {
                
                    

                    Guild? guild = (from g in messageEvent.db.Guilds
                                where g.GuildID == discord_guild.Id.ToString()
                                select g).FirstOrDefault();

                    if(guild == null)
                    {
                        
                        logger.Information($"Registering guild: {discord_guild.Id}");

                        messageEvent.db.Add(new Guild(discord_guild.Name, discord_guild.Id.ToString()));
                        messageEvent.db.SaveChanges();

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
        
        [CommandPermissionRequired(PermissionLevels.Admin)]
        private async Task SetRole(CommandContext<CommandArguments> ctx, MessageCreatedEvent messageEvent)
        {
            if (messageEvent.guild == null)
            {
                messageEvent.RespondError("This command can only be run in a server");
            }
            
            var levelName = Arguments.GetString(ctx,"level");
            if (Enum.TryParse(levelName, out PermissionLevels level))
            {
                
                await messageEvent.db.Entry(messageEvent.guild)
                    .Collection(g => g!.PermissionLevels).LoadAsync();
                
                var role = ctx.GetArgument<DiscordRole>("role");

                var setting = (from p in messageEvent.guild!.PermissionLevels
                    where p.PermissionLevel == level && p.RoleID == role.Id.ToString()
                        select p).FirstOrDefault();
                if (setting == null)
                {
                    messageEvent.guild!.PermissionLevels.Add(new GuildPermissionLevel(role.Id.ToString(),level));    
                    messageEvent.Respond("Done");
                }
                else
                {
                    messageEvent.Respond("Already added");
                }
               

                await messageEvent.db.SaveChangesAsync();
            }
        }
        
        private async Task RemoveRole(CommandContext<CommandArguments> ctx, MessageCreatedEvent messageEvent)
        {
            if (messageEvent.guild == null)
            {
                messageEvent.RespondError("This command can only be run in a server");
            }
            
            var levelName = Arguments.GetString(ctx,"level");
            var role = ctx.GetArgument<DiscordRole>("role");
            if (Enum.TryParse(levelName, out PermissionLevels level))
            {
                
                await messageEvent.db.Entry(messageEvent.guild)
                    .Collection(g => g!.PermissionLevels).LoadAsync();
                
                var setting = (from p in messageEvent.guild!.PermissionLevels
                    where p.PermissionLevel == level && p.RoleID == role.Id.ToString()
                    select p).FirstOrDefault();
                
                if (setting != null)
                {
                    messageEvent.guild!.PermissionLevels.Remove(setting);    
                    messageEvent.Respond("Done");
                }
                else
                {
                    messageEvent.Respond("Didn't find");
                }
               

                await messageEvent.db.SaveChangesAsync();
            }
        }

    }
}
