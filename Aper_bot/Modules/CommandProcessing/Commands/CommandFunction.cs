using System;
using System.Reflection;
using System.Threading.Tasks;
using Aper_bot.Events;
using Mars;

namespace Aper_bot.Modules.CommandProcessing
{
    /// <summary>
    /// This is the callback that is supplied to Mars and later retreated after the command resolution
    /// It contains 2 possible command function signatures, one for async and one for normal execution.
    /// </summary>
    public class CommandFunction : Mars.CommandCallback
    {
        public delegate int Command(Mars.ParseResult result,IMessageCreatedEvent discordMessageEvent);
        public delegate Task AsyncCommand(Mars.ParseResult result,IMessageCreatedEvent discordMessageEvent);

        public Command Cmd;
        public readonly MethodInfo cmdMeta;

        public CommandFunction(Command cmd)
        {
            this.Cmd = cmd;

            this.cmdMeta = cmd.Method;

        }
        
        public CommandFunction(AsyncCommand cmd)
        {
            this.Cmd = ((result, @event) =>
            {
                cmd(result, @event).Wait();
                return 0;
            });
            
            this.cmdMeta = cmd.Method;
        }
    }
    
    public static class CommandFunctionHelpers{
    
        public static CommandNode ThisCalls(this CommandNode c, CommandFunction.AsyncCommand cmd)
        {
            return c.ThisCalls(new CommandFunction(cmd));
        }
        
        public static CommandNode ThisCalls(this CommandNode c, CommandFunction.Command cmd)
        {
            return c.ThisCalls(new CommandFunction(cmd));
        }
    }
}