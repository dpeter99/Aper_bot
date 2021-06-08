using System;

namespace Aper_bot.Modules.CommandProcessing.Attributes
{
    public abstract class CommandAttribute : Attribute, ICommandConditionProvider 
    {
        public abstract Type GetCondition();
    }
}