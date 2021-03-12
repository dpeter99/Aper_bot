using Brigadier.NET.Builder;
using Brigadier.NET.Tree;

namespace Aper_bot.Modules.CommandProcessing
{
    public static class BrigadierExtensions
    {
        public static TThis Executes<TThis, TNode>(this ArgumentBuilder<CommandExecutionContext, TThis, TNode> t, CommandExecutionContext.Command command) 
            where TThis : ArgumentBuilder<CommandExecutionContext, TThis, TNode> 
            where TNode : CommandNode<CommandExecutionContext>
        {
            return t.Executes( (ctx) =>
            {
                //ctx.Source.exectutionTask = command(ctx, ctx.Source.Event);
                ctx.Source.ctx = ctx;
                ctx.Source.command = command;
                return 0;
            });
        }
    }
}