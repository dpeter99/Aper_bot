using Aper_bot.Database;
using Aper_bot.Database.Model;
using Aper_bot.Modules.Commands;

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

namespace Aper_bot.Modules
{
    [CommandProvider]
    class ServerSettingsCommand : ChatCommands
    {
        IDbContextFactory<DatabaseContext> dbContextFactory;
        ILogger logger;
        IOptions<Config> config;

        public ServerSettingsCommand(IDbContextFactory<DatabaseContext> db, ILogger log, IOptions<Config> options)
        {
            dbContextFactory = db;
            logger = log;
            config = options;
        }

        public override LiteralArgumentBuilder<CommandSourceStack> Register(IArgumentContext<CommandSourceStack> l)
        {
            return l.Literal("/server")
                .Then(
                    s =>
                    {
                        return s.Literal("setup")
                                .Executes(AsyncExecue(ServerSetup));
                    }
                );
        }

        private async Task ServerSetup(CommandContext<CommandSourceStack> context)
        {
            var message = context.Source.@event;

            DSharpPlus.Entities.DiscordGuild discord_guild = message.Guild;

            if (context.Source.guild != null)
            {
                await message.Message.RespondAsync("Server already active");
                return;
            }

            var member = await message.Guild.GetMemberAsync(message.Message.Author.Id);

            var permissions = member.PermissionsIn(message.Message.Channel);
            if (permissions.HasPermission(Permissions.Administrator) || member.Id.ToString() == config.Value.Owner)
            {
                using (var db = dbContextFactory.CreateDbContext())
                {
                    

                    Guild? guild = (from g in db.Guilds
                                where g.GuildID == discord_guild.Id.ToString()
                                select g).FirstOrDefault();

                    if(guild == null)
                    {
                        
                        logger.Information($"Registering guild: {discord_guild.Id}");

                        db.Add(new Guild(discord_guild.Name, discord_guild.Id.ToString()));
                        db.SaveChanges();

                        var embed = new DSharpPlus.Entities.DiscordEmbedBuilder()
                        {
                            Description = "Server setup complete!"
                        };

                        await message.Message.RespondAsync(embed: embed.Build());
                    }

                }
            }
            else
            {
                await message.Message.RespondAsync("You do not have the permission");
            }
        }
    }
}
