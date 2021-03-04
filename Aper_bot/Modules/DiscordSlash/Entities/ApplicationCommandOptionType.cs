using System;
using DSharpPlus.Entities;

namespace Aper_bot.Modules.DiscordSlash.Entities
{
    public enum ApplicationCommandOptionType
    {
        SubCommand = 1,
        SubCommandGroup = 2,
        String = 3,
        Integer = 4,
        Boolean = 5,
        User = 6,
        Channel = 7,
        Role = 8
    }

    public static class ApplicationCommandOptionTypeExtensions
    {
        public static ApplicationCommandOptionType? GetOptionType(Type parameter)
        {
            if (parameter == typeof(string))
                return ApplicationCommandOptionType.String;
            else if (parameter == typeof(int))
                return ApplicationCommandOptionType.Integer;
            else if (parameter == typeof(bool))
                return ApplicationCommandOptionType.Boolean;
            else if (parameter == typeof(DiscordUser))
                return ApplicationCommandOptionType.User;
            else if (parameter == typeof(DiscordChannel))
                return ApplicationCommandOptionType.Channel;
            else if (parameter == typeof(DiscordRole))
                return ApplicationCommandOptionType.Role;
            else
                return null;
        }
    }
}
