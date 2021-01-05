using DSharpPlus.Entities;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aper_bot.Database.Model
{
    public class User : Entity
    {
        /// <summary>
        /// The name of the user
        /// </summary>
        /// TODO: This might differ form server to server this should be somehow handeled
        [Required]
        public string Name { get; set; } = "";

        [Required]
        public string UserID { get; set; } = "";

        [InverseProperty("Source")]
        public List<Quote> quotes { get; set; } = new List<Quote>();

        [InverseProperty("Creator")]
        public List<Quote> quotesMade { get; set; } = new List<Quote>();

        public User(string name, string userID)
        {
            Name = name;
            UserID = userID;
        }

        public User(DiscordUser discordUser)
        {
            Name = discordUser.Username;
            UserID = discordUser.Id.ToString();
        }
    }
}