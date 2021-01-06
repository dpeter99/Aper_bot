using Aper_bot.Events;

using Brigadier.NET;
using Brigadier.NET.Builder;
using Brigadier.NET.Context;

using System.Threading.Tasks;

namespace Aper_bot.Modules.Commands
{
    internal abstract class ChatCommands
    {
        public abstract LiteralArgumentBuilder<CommandArguments> Register(IArgumentContext<CommandArguments> l);

        protected Command<CommandArguments> AsyncExecue(AsyncCommand command)
        {
            return (ctx) =>
            {
                ctx.Source.exectutionTask = command(ctx, ctx.Source.Event);
                return 0;
            };
        }

        protected delegate Task AsyncCommand(CommandContext<CommandArguments> ctx, MessageCreatedEvent messageEvent);
    }
}