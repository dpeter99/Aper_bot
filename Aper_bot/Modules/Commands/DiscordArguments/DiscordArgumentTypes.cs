using Brigadier.NET;
using Brigadier.NET.Context;

using DSharpPlus.Entities;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Aper_bot.Modules.Commands.DiscordArguments
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
