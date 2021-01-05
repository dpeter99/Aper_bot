using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Aper_bot.Database.Model
{
    public class Quote : Entity
    {

        public int number { get; set; }

        [Required]
        public int CreatorID { get; set; }
        private User? _creator;
        /// <summary>
        /// The user who created the quote
        /// </summary>
        [Required]       
        public User Creator
        {
            set => _creator = value;
            get => _creator
                   ?? throw new InvalidOperationException("Uninitialized property: " + nameof(Creator));
        }


        public int? SourceID { get; set; }
        private User? _source;
        /// <summary>
        /// Quotee
        /// </summary>
        public User? Source
        {
            set => _source = value;
            get => _source
                   ?? throw new InvalidOperationException("Uninitialized property: " + nameof(Source));
        }

        public int GuildID { get; set; }
        private Guild? _guild;
        public Guild Guild
        {
            set => _guild = value;
            get => _guild
                   ?? throw new InvalidOperationException("Uninitialized property: " + nameof(Source));
        }


        /// <summary>
        /// When the quote was created
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// When the actual quote happen
        /// </summary>
        public DateTime EventTime { get; set; }

        /// <summary>
        /// The text of the quote
        /// </summary>
        public string Text { get; set; }


        public Image? Image { get; set; }

        public Quote(DateTime creationTime, DateTime eventTime, string text)
        {
            CreationTime = creationTime;
            EventTime = eventTime;
            Text = text;
        }

        public Quote(int CreatorID, int? SourceID, int guildID, DateTime creation, DateTime eventTime, string text, Image? Image)
        {
            this.CreatorID = CreatorID;
            this.SourceID = SourceID;
            this.GuildID = guildID;
            CreationTime = creation;
            EventTime = eventTime;
            Text = text;
            this.Image = Image;
        }
    }
}
