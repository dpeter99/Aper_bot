using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aper_bot.Database.Model
{
    public class Guild: Entity
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string GuildID { get; set; }

        public List<GuildRule> Rules { get; set; } = new List<GuildRule>();

        public Guild(string name, string GuildID)
        {
            Name = name;
            this.GuildID = GuildID;
        }
    }

}
