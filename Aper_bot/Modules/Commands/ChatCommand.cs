using Brigadier.NET.Builder;

namespace Aper_bot.Modules.Commands
{
    internal abstract class ChatCommand
    {
        public abstract LiteralArgumentBuilder<CommandSourceStack> Register(IArgumentContext<CommandSourceStack> l);
    }
}