using System.Collections.Generic;
using Aper_bot.Modules.Discord.SlashCommands.Entities;

namespace Aper_bot.Modules.Discord.SlashCommands
{
    public interface ISlashCommandSuplier
    {
        public IEnumerable<ApplicationCommand> GetCommands();
    }
}