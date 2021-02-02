using Brigadier.NET;

namespace Aper_bot.Modules.CommandProcessing
{
    public interface ICommandHandler
    {
        void ExecuteCommand(CommandArguments context);

        CommandDispatcher<CommandArguments> dispatcher { get; }
    }
}