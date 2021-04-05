using Aper_bot.Events;

namespace Aper_bot.Modules.CommandProcessing.Commands
{
    public interface ICommandProcessor
    {
        Mars.CommandTree tree { get; }

        void ProcessMessage(IMessageCreatedEvent messageEvent);

        void RunCommand(CommandExecutionContext ctx);
    }
}