using Aper_bot.Modules.Discord;
using Brigadier.NET.Context;
using DSharpPlus.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Aper_bot.Modules.CommandProcessing.DiscordArguments
{
    public static class DiscordArgumentTypes
    {
        internal static DiscordUserArgumentType User()
        {
            var i = ActivatorUtilities.CreateInstance<DiscordUserArgumentType>(Application.Instance.host.Services,new object[] { DiscordBot.Instance });

            return i;
        }
        
        
        
        internal static DiscordRoleArgumentType Role()
        {
            var i = ActivatorUtilities.CreateInstance<DiscordRoleArgumentType>(Application.Instance.host.Services,new object[] { DiscordBot.Instance });

            return i;
        }


        public static DiscordUser GetUser<TSource>(CommandContext<TSource> context, string name)
        {
            return context.GetArgument<DiscordUser>(name);
        }
    }
}
