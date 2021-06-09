using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aper_bot.Util;
using Aper_bot.Util.Discord;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aper_bot.Database.Model
{
    [Table("Guild", Schema = CoreDatabaseContext.Schema)]
    public class Guild: Entity
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public Snowflake GuildID { get; set; }

        public List<GuildPermissionLevel> PermissionLevels { get; set; } = new List<GuildPermissionLevel>();

        public List<GuildRule> Rules { get; set; } = new List<GuildRule>();

        public Snowflake? RulesMessageId { get; set; }
        public Snowflake? RulesChannelId { get; set; }

        // ReSharper disable once CollectionNeverUpdated.Global
        public List<Quote> Quotes { get; set; } = new List<Quote>();

        public Guild(string name, Snowflake GuildID)
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
                
                builder
                    .Property(x => x.GuildID)
                    .HasConversion(Snowflake.GetConverter())
                    .IsRequired();
            }
        }
        
    }

}
