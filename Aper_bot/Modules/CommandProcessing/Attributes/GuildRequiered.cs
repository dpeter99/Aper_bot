using System;
using System.Threading.Tasks;
using Aper_bot.Events;
using Aper_bot.Modules.Discord;

namespace Aper_bot.Modules.CommandProcessing.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    sealed class GuildRequiered : CommandAttribute
    {
        private bool _setup;

        public bool Setup
        {
            get => _setup;
            set => _setup = value;
        }
        
        public GuildRequiered(bool setup = true)
        {
            _setup = setup;
        }
        
        public override Type GetCondition()
        {
            return typeof(Condition);
        }
        

        public class Condition : CommandCondition
        {
            public override async Task<bool> CheckCondition(CommandFunction func, IMessageCreatedEvent context, ICommandConditionProvider provider)
            {
                var atribute = provider as GuildRequiered;

                if (atribute == null)
                    throw new Exception("Bad input");
                

                if (atribute.Setup)
                {
                    return context.Guild != null;
                }
                else
                {
                    return ((DiscordMessageCreatedEvent)context).@event.Channel.IsPrivate == false;
                }

                
            }
        }
    }
}
