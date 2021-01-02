using Brigadier.NET;
using Brigadier.NET.ArgumentTypes;

using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Aper_bot.Modules.Commands.DiscordArguments
{
    class DiscordUserArgumentType : ArgumentType<DiscordUser>
    {
        private static Regex UserRegex { get; }

        static DiscordUserArgumentType()
        {
            UserRegex = new Regex(@"^<@\!?(\d+?)>$", RegexOptions.ECMAScript | RegexOptions.Compiled);
        }

        public override DiscordUser Parse(IStringReader reader)
        {
            string token = reader.ReadString();

            if (ulong.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out var uid))
            {
                var result = await ctx.Client.GetUserAsync(uid).ConfigureAwait(false);
                var ret = result != null ? Optional.FromValue(result) : Optional.FromNoValue<DiscordUser>();
                return ret;
            }
        }
    }
}
