using System.Threading.Tasks;
using Aper_bot.Events;
using Aper_bot.Modules.Discord;
using Brigadier.NET;
using Brigadier.NET.Builder;
using Brigadier.NET.Context;

namespace Aper_bot.Modules.CommandProcessing
{
    public abstract class ChatCommands
    {
        public abstract LiteralArgumentBuilder<CommandExecutionContext> Register(IArgumentContext<CommandExecutionContext> l);

        protected Command<CommandExecutionContext> AsyncExecue(CommandExecutionContext.Command command)
        {
            return (ctx) =>
            {
                //ctx.Source.exectutionTask = command(ctx, ctx.Source.Event);
                ctx.Source.ctx = ctx;
                ctx.Source.command = command;
                return 0;
            };
        }

        protected delegate Task AsyncCommand(CommandContext<CommandExecutionContext> ctx, DiscordMessageCreatedEvent discordMessageEvent);
    }
}