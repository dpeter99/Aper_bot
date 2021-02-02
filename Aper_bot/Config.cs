using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aper_bot
{
    public class Config
    {
        public bool CaseSensitive { get; set; }
        public string Owner { get; set; } = "";

        public string? DiscordBotKey { get; set; }
        
        public string? PublicKey { get; set; }
        
        public string? BotID { get; set; }
    }
}
