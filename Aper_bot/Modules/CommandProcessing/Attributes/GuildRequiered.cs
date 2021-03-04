using System;
using System.Threading.Tasks;
using Aper_bot.Events;

namespace Aper_bot.Modules.CommandProcessing.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    sealed class GuildRequiered : Attribute, ICommandConditionProvider
    {
        private bool _setup;

        public bool Setup
        {
            get => _setup;
            set => _setup = value;
        }
        
        public GuildRequiered(bool setup)
        {
            _setup = setup;
        }
        
        public Type GetCondition(CommandExecutionContext context)
        {
            return typeof(Condition);
        }
        

        public class Condition : CommandCondition
        {
            public override async Task<bool> CheckCondition(CommandExecutionContext context, ICommandConditionProvider p)
            {
                var atribute = p as GuildRequiered;

                if (atribute == null)
                    throw new Exception("Bad input");
                
                var db = context.db;

                if (atribute.Setup)
                {
                    return context.Event.Guild != null;
                }
                else
                {
                    return ((DiscordMessageCreatedEvent)context.Event).@event.Channel.IsPrivate == false;
                }

                
            }
        }
    }
}
