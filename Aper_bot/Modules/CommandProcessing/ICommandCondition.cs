using System;
using System.Threading.Tasks;

namespace Aper_bot.Modules.CommandProcessing
{
    internal interface ICommandConditionProvider
    {
        Type GetCondition(CommandExecutionContext context);
    }
    
    internal abstract class CommandCondition
    {
        public abstract Task<bool> CheckCondition(CommandExecutionContext context, ICommandConditionProvider provider);
    }
}