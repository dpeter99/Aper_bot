using System.ComponentModel.DataAnnotations.Schema;
using Aper_bot.Database.Model;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Aper_bot.Modules.DiscordSlash.Database.Model
{
    [Table("Commands", Schema = "SlashCommands")]
    public class Command: Entity
    {
        public string Name { get; set; }

        public string CommandID { get; set; }

        public int Version { get; set; }

        
        public Guild Guild { get; set; }

        public Command(string name, string commandID, int version)
        {
            Name = name;
            CommandID = commandID;
            Version = version;
        }
    }
}