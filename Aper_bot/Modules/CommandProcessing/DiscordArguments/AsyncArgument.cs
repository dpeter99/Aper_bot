using System;
using System.Threading.Tasks;
using Mars;

namespace Aper_bot.Modules.CommandProcessing.DiscordArguments
{
    abstract class AsyncArgument<T> : ArgumentType<T>
    {
        public override T Parse(StringReader reader, ParseResult result)
        {
            Task<T> task = Process(reader, result);

            try
            {
                task.Wait();
            }
            catch (AggregateException e)
            {
                throw e.InnerException ?? e;
            }

            return task.Result;
        }
        
        public abstract Task<T> Process(StringReader reader, ParseResult result);
        
    }
}
