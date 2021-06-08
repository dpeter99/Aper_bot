using System.Threading.Tasks;
using Aper_bot.Events;
using Mars;

namespace Aper_bot.Modules.CommandProcessing.Commands
{
    public interface ICommandExecutor
    {
        

        void ProcessMessage(IMessageCreatedEvent messageEvent);

        Task RunCommand(ParseResult result, IMessageCreatedEvent msgEvent);
    }
}