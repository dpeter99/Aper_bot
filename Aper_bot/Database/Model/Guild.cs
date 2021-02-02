using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aper_bot.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aper_bot.Database.Model
{
    public class Guild: Entity
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string GuildID { get; set; }

        public List<GuildPermissionLevel> PermissionLevels { get; set; } = new List<GuildPermissionLevel>();

        public List<GuildRule> Rules { get; set; } = new List<GuildRule>();

        public Snowflake? RulesMessageId { get; set; }
        public Snowflake? RulesChannelId { get; set; }

        // ReSharper disable once CollectionNeverUpdated.Global
        public List<Quote> Quotes { get; set; } = new List<Quote>();

        public Guild(string name, string GuildID)
        {
            Name = name;
            this.GuildID = GuildID;
        }

        class Configuration:IEntityTypeConfiguration<Guild>
        {
            public void Configure(EntityTypeBuilder<Guild> builder)
            {
                builder
                    .Property(x => x.RulesMessageId)
                    .HasConversion(Snowflake.GetConverter());
                
                builder
                    .Property(x => x.RulesChannelId)
                    .HasConversion(Snowflake.GetConverter());
            }
        }
        
    }

}
