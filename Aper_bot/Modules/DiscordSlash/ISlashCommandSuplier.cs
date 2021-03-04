using System.Collections.Generic;
using Aper_bot.Modules.DiscordSlash.Entities;

namespace Aper_bot.Modules.DiscordSlash
{
    public interface ISlashCommandSuplier
    {
        public IEnumerable<SlashCommand> GetCommands();
    }
}