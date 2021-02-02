using System.Threading.Tasks;
using Aper_bot.Events;
using Brigadier.NET;
using Brigadier.NET.Builder;
using Brigadier.NET.Context;

namespace Aper_bot.Modules.CommandProcessing
{
    internal abstract class ChatCommands
    {
        public abstract LiteralArgumentBuilder<CommandArguments> Register(IArgumentContext<CommandArguments> l);

        protected Command<CommandArguments> AsyncExecue(CommandArguments.Command command)
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