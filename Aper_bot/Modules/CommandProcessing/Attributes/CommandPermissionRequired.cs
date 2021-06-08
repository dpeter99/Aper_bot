using System;
using System.Linq;
using System.Threading.Tasks;
using Aper_bot.Events;
using Aper_bot.Modules.Discord;
using Aper_bot.Modules.Discord.Config;
using Aper_bot.Util.Discord;
using Microsoft.Extensions.Options;

namespace Aper_bot.Modules.CommandProcessing.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    sealed class CommandPermissionRequired : CommandAttribute
    {
        PermissionLevels _permission;
        
        public PermissionLevels Permission
        {
            get { return _permission; }
        }
        
        public CommandPermissionRequired(PermissionLevels permission)
        {
            this._permission = permission;
        }

        public override Type GetCondition()
        {
            return typeof(Condition);
        }
        

        public class Condition : CommandCondition
        {
            private readonly IOptions<DiscordConfig> _conf;
            private readonly DiscordBot _bot;

            public Condition(IOptions<DiscordConfig> conf, DiscordBot bot)
            {
                _conf = conf;
                _bot = bot;
            }
            
            public override async Task<bool> CheckCondition(CommandFunction func, IMessageCreatedEvent context, ICommandConditionProvider provider)
            {
                var attribute = (CommandPermissionRequired)provider;
                
                var db = context.Db;
            
                if(context.Guild is not null)
                {
                    db.Entry(context.Guild).Collection(g => g.PermissionLevels).LoadAsync().Wait();

                    //var member = await ((DiscordMessageCreatedEvent)context).@event.Guild.GetMemberAsync(ulong.Parse(context.Author.UserID));

                    var DGuild = await _bot.Client.GetGuildAsync(new Snowflake(context.Guild.GuildID));
                    

                    PermissionLevels memberLevel = PermissionLevels.None;

                    if (context.Author.UserID == _conf.Value.Owner ||
                        context.Author.UserID == DGuild.OwnerId.ToString())
                    {
                        memberLevel = PermissionLevels.Owner;
                    }
                    
                    var member = await DGuild.GetMemberAsync(new Snowflake(context.Author.UserID));
                    
                    foreach (var guildLevel in context.Guild.PermissionLevels)
                    {
                        if (member.Roles.Any(r => r.Id.ToString() == guildLevel.RoleID))
                        {
                            if (guildLevel.PermissionLevel > memberLevel)
                            {
                                memberLevel = guildLevel.PermissionLevel;
                            }
                        }
                    }

                    if (attribute._permission > memberLevel)
                    {
                        context.RespondError($"You do not have the needed permissions. \n You must be {attribute._permission.ToString()} or above");
                        return false;
                    }

                    return true;
                }

                return false;
            }
        }
    }
}
