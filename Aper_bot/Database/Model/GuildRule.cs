using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aper_bot.Database.Model
{
    [Table("GuildRules", Schema = CoreDatabaseContext.Schema)]
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
