using Brigadier.NET;
using Brigadier.NET.ArgumentTypes;

using DSharpPlus.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aper_bot.Modules.Commands.DiscordArguments
{
    abstract class AsyncArgument<T> : ArgumentType<T>
    {
        public override T Parse(IStringReader reader)
        {
            return DoParse<object>(reader, null);
        }

        public override T Parse<TSource>(IStringReader reader, TSource source)
        {
            return DoParse(reader, source);
        }

        public T DoParse<TSource>(IStringReader reader, TSource? source)
        {
            Task<T> task = Process(reader, source);

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

        public abstract Task<T> Process<TSource>(IStringReader reader, TSource source);
        
    }
}
