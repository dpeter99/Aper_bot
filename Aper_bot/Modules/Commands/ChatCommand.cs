using Aper_bot.Events;

using Brigadier.NET;
using Brigadier.NET.Builder;
using Brigadier.NET.Context;

using System.Threading.Tasks;

using static Aper_bot.Modules.Commands.CommandArguments;

namespace Aper_bot.Modules.Commands
{
    internal abstract class ChatCommands
    {
        public abstract LiteralArgumentBuilder<CommandArguments> Register(IArgumentContext<CommandArguments> l);

        protected Command<CommandArguments> AsyncExecue(Command command)
        {
            return (ctx) =>
            {
                //ctx.Source.exectutionTask = command(ctx, ctx.Source.Event);
                ctx.Source.ctx = ctx;
                ctx.Source.exectutionTask = command;
                return 0;
            };
        }

        protected delegate Task AsyncCommand(CommandContext<CommandArguments> ctx, MessageCreatedEvent messageEvent);
    }
}