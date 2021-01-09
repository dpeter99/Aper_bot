using System;
using System.Threading.Tasks;

namespace Aper_bot.Modules.Commands
{
    internal interface ICommandConditionProvider
    {
        Type GetCondition(CommandArguments context);
    }
    
    internal abstract class CommandCondition
    {
        public abstract Task<bool> CheckCondition(CommandArguments context, ICommandConditionProvider provider);
    }
}