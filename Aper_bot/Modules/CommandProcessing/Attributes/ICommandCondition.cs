using System;
using System.Threading.Tasks;
using Aper_bot.Events;

namespace Aper_bot.Modules.CommandProcessing
{
    internal interface ICommandConditionProvider
    {
        Type GetCondition();
    }
    
    internal abstract class CommandCondition
    {
        public abstract Task<bool> CheckCondition(CommandFunction func, IMessageCreatedEvent context, ICommandConditionProvider provider);
    }
}