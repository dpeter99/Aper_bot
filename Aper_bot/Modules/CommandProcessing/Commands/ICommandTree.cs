using Aper_bot.Events;
using Brigadier.NET;

namespace Aper_bot.Modules.CommandProcessing.Commands
{
    public interface ICommandTree
    {
        CommandDispatcher<CommandExecutionContext> dispatcher { get; }

        void ProcessMessage(IMessageCreatedEvent messageEvent);

        void RunCommand(CommandExecutionContext ctx);
    }
}