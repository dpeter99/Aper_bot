using System.Collections.Generic;
using System.Threading.Tasks;
using Aper_bot.Events;
using Aper_bot.Modules.Discord;
using Mars;

namespace Aper_bot.Modules.CommandProcessing
{
    /// <summary>
    /// Base class for all the command
    /// </summary>
    public abstract class ChatCommands
    {
        /// <summary>
        /// Used to register a list of commands.
        /// </summary>
        /// <returns>The commands this class registers</returns>
        public abstract IEnumerable<CommandNode> Register();
        
    }
}