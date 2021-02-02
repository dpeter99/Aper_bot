using Brigadier.NET.Builder;
using Brigadier.NET.Tree;

namespace Aper_bot.Modules.CommandProcessing
{
    public static class BrigadierExtensions
    {


        public static TThis Executes<TThis, TNode>(this ArgumentBuilder<CommandArguments, TThis, TNode> t, CommandArguments.Command command) 
            where TThis : ArgumentBuilder<CommandArguments, TThis, TNode> 
            where TNode : CommandNode<CommandArguments>
        {
            return t.Executes( (ctx) =>
            {
                //ctx.Source.exectutionTask = command(ctx, ctx.Source.Event);
                ctx.Source.ctx = ctx;
                ctx.Source.exectutionTask = command;
                return 0;
            });
        }
    }
}