using System.Collections.Generic;

namespace Aper_bot.Modules.WebAPI.V1.Model
{
    public class QuotesData
    {
        public Dictionary<string,User> Users = new ();

        public List<Quote> Quotes = new ();
        
    }

    public class Quote
    {
        public string text = "";
        public string user = "";

        public Quote(string text, string user)
        {
            this.text = text;
            this.user = user;
        }
    }

    public class User
    {
        public string id = "";
        
        public string username = "";

        public string avatar = "https://cdn.discordapp.com/avatars/140245257416736769/7ac3052f54cd37f2f7941276f4da1d18.webp";

        public User(string id, string username, string avatar)
        {
            this.id = id;
            this.username = username;
            this.avatar = avatar;
        }
    }
}