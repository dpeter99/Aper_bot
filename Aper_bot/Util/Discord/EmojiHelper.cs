using Aper_bot.Modules.Discord;
using DSharpPlus.Entities;

namespace Aper_bot.Util.Discord
{
    public static class EmojiHelper
    {
        public static string Number(int i)
        {
            var si = NumberToName.GetIntArray(i);
            var res = "";
            
            for (int j = 0; j < si.Length; j++)
            {
                res += DiscordEmoji.FromName(DiscordBot.Instance.Client, $":{si[j].ToName()}:");
            }

            
            
            return res;
        }

        public static string Cross()
        {
            return DiscordEmoji.FromName(DiscordBot.Instance.Client, $":x:");
        }
    }
}