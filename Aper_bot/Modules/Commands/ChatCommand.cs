using Brigadier.NET;
using Brigadier.NET.Builder;
using Brigadier.NET.Context;

using System.Threading.Tasks;

namespace Aper_bot.Modules.Commands
{
    internal abstract class ChatCommands
    {
        public abstract LiteralArgumentBuilder<CommandSourceStack> Register(IArgumentContext<CommandSourceStack> l);

        protected Command<CommandSourceStack> AsyncExecue(AsyncCommand command)
        {
            return (ctx) =>
            {
                ctx.Source.exectutionTask = command(ctx);
                return 0;
            };
        }

        protected delegate Task AsyncCommand(CommandContext<CommandSourceStack> ctx);
    }
}