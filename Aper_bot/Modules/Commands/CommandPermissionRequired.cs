using DSharpPlus;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Aper_bot.Modules.Commands
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

        public Type GetCondition(CommandArguments context)
        {
            return typeof(Condition);
        }
        

        public class Condition : CommandCondition
        {
            private readonly IOptions<Config> _conf;

            public Condition(IOptions<Aper_bot.Config> conf)
            {
                _conf = conf;
            }
            
            public override async Task<bool> CheckCondition(CommandArguments context, ICommandConditionProvider p)
            {
                var atribute = (CommandPermissionRequired)p;
                
                var db = context.Event.db;
            
                if(context.Event.guild != null)
                {
                    db.Entry(context.Event.guild)
                        .Collection(g => g.PermissionLevels).LoadAsync().Wait();

                    var member = await context.Event.@event.Guild.GetMemberAsync(ulong.Parse(context.Event.author.UserID));

                    PermissionLevels memberLevel = PermissionLevels.None;

                    if (member.Id.ToString() == _conf.Value.Owner ||
                        member.Id.ToString() == context.Event.@event.Guild.OwnerId.ToString())
                    {
                        memberLevel = PermissionLevels.Owner;
                    }
                    
                    
                
                    foreach (var guildLevel in context.Event.guild.PermissionLevels)
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
