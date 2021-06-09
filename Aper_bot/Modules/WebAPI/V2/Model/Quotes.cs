using System.Collections.Generic;
using Newtonsoft.Json;

namespace Aper_bot.Modules.WebAPI.V2.Model
{
    /// <summary>
    /// Grouping object for storing all the response data
    /// </summary>
    public class QuotesData
    {
        /// <summary>
        /// List of all the users who were quoted
        /// </summary>
        public Dictionary<string, UserData> Users { get; set; } = new ();
        
        /// <summary>
        /// List of all the roles used by the users
        /// </summary>
        public Dictionary<string, RoleData> Roles { get; set; } = new ();

        /// <summary>
        /// List of all the quotes
        /// </summary>
        public List<QuoteData> Quotes { get; set; } = new ();
        
    }

    /// <summary>
    /// Represents a single quote from a user
    /// </summary>
    public class QuoteData
    {
        /// <summary>
        /// The text of the quote
        /// </summary>
        public string Text { get; set; }
        
        /// <summary>
        /// The user who said the quote
        /// </summary>
        /// <remarks>
        /// This is not the person who made the quote. But the one being quoted
        /// </remarks>
        public string User { get; set; }

        /// <summary>
        /// Default Constructor for setting all the required properties
        /// </summary>
        /// <param name="text"></param>
        /// <param name="user"></param>
        public QuoteData(string text, string user)
        {
            this.Text = text;
            this.User = user;
        }
    }

    public class RoleData
    {
        public string Color { get; set; }
        
        public string Name { get; set; }

        public RoleData(string color, string name)
        {
            Color = color;
            Name = name;
        }
    }
    
    /// <summary>
    /// Data about a specific user
    /// </summary>
    public class UserData
    {
        /// <summary>
        /// The discord id of the user
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The discord username if the user
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// A link to the user's discord profile picture
        /// </summary>
        public string Avatar { get; set; } = "https://cdn.discordapp.com/avatars/140245257416736769/7ac3052f54cd37f2f7941276f4da1d18.webp";

        /// <summary>
        /// The #1111 part of the user's name
        /// </summary>
        [JsonProperty("discriminator")]
        public int Discriminator { get; set; }

        
        /// <summary>
        /// The names of the roles the user has
        /// </summary>
        [JsonProperty("roles")]
        public List<string> Roles { get; set; }

        /// <summary>
        /// Constructor the sets all the required properties
        /// </summary>
        /// <param name="id">The Id for the user</param>
        /// <param name="username">User name for the user</param>
        /// <param name="avatar">Avatar of the user</param>
        /// <param name="disc"></param>
        /// <param name="roles"></param>
        public UserData(string id, string username, string avatar, int disc, List<string> roles)
        {
            this.Id = id;
            this.Username = username;
            this.Avatar = avatar;
            Discriminator = disc;
            Roles = roles;
        }
    }
}