using System;
using System.Linq;
using System.Threading.Tasks;
using Aper_bot.Events;
using Aper_bot.Modules.Discord;
using Aper_bot.Modules.Discord.Config;
using Microsoft.Extensions.Options;

namespace Aper_bot.Modules.CommandProcessing.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    sealed class CommandPermissionRequired : Attribute, ICommandConditionProvider
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

        public Type GetCondition(CommandExecutionContext context)
        {
            return typeof(Condition);
        }
        

        public class Condition : CommandCondition
        {
            private readonly IOptions<DiscordConfig> _conf;

            public Condition(IOptions<DiscordConfig> conf)
            {
                _conf = conf;
            }
            
            public override async Task<bool> CheckCondition(CommandExecutionContext context, ICommandConditionProvider p)
            {
                var atribute = (CommandPermissionRequired)p;
                
                var db = context.Db;
            
                if(context.Event.Guild != null)
                {
                    db.Entry(context.Event.Guild)
                        .Collection(g => g.PermissionLevels).LoadAsync().Wait();

                    var member = await ((DiscordMessageCreatedEvent)context.Event).@event.Guild.GetMemberAsync(ulong.Parse(context.Event.Author.UserID));

                    PermissionLevels memberLevel = PermissionLevels.None;

                    if (member.Id.ToString() == _conf.Value.Owner ||
                        member.Id.ToString() == ((DiscordMessageCreatedEvent)context.Event).@event.Guild.OwnerId.ToString())
                    {
                        memberLevel = PermissionLevels.Owner;
                    }
                    
                    
                
                    foreach (var guildLevel in context.Event.Guild.PermissionLevels)
                    {
                        if (member.Roles.Any(r => r.Id.ToString() == guildLevel.RoleID))
                        {
                            if (guildLevel.PermissionLevel > memberLevel)
                            {
                                memberLevel = guildLevel.PermissionLevel;
                            }
                        }
                    }

                    if (atribute._permission > memberLevel)
                    {
                        context.Event.RespondError($"You do not have the needed permissions. \n You must be {atribute._permission.ToString()} or above");
                        return false;
                    }

                    return true;
                }

                return false;
            }
        }
    }
}
