using Aper_bot.Modules.Discord;
using DSharpPlus.Entities;

namespace Aper_bot.Util.Discord
{
    public static class EmojiHelper
    {
        public static string Number(int i)
        {
            return DiscordEmoji.FromName(DiscordBot.Instance.Client, $":{i.ToName()}:");
        }

        public static string Cross()
        {
            return DiscordEmoji.FromName(DiscordBot.Instance.Client, $":x:");
        }
    }
}