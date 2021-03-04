using Aper_bot.Modules.CommandProcessing;

namespace Aper_bot.CommandExecutor
{
    public interface ICommandExecutor
    {
        void ExecuteCommand(CommandExecutionContext commandExecutionContext);
    }
}