using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aper_bot.Database.Model
{
    public class GuildRule: Entity
    {
        public int Number { get; set; }

        public string Description { get; set; }

        public GuildRule(int number, string description)
        {
            Number = number;
            Description = description;
        }
    }
}
