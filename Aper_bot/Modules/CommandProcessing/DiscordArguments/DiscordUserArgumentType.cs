using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Aper_bot.Modules.Discord;
using Aper_bot.Util;
using DSharpPlus.Entities;
using Mars;
using Microsoft.Extensions.Options;

namespace Aper_bot.Modules.CommandProcessing.DiscordArguments
{
    
    class DiscordUserArgumentType : AsyncArgument<DiscordUser>
    {
        private static readonly IEnumerable<string> UserExamples = new[] { "@username", "794664673487749131", "..." };

        private static Regex UserRegex { get; } = new (@"^<@\!?(\d+?)>$", RegexOptions.ECMAScript | RegexOptions.Compiled);

        Config config;

        DiscordBot discord;

        public DiscordUserArgumentType(IOptions<Config> conf)
        {
            config = conf.Value;
            discord = DiscordBot.Instance;
        }

        public override bool CanParse(string text)
        {
            var token = text.Split(' ', 2)[0];
            return ulong.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out var uid) ||
                   UserRegex.Match(token).Success;
        }
        
        public override async Task<DiscordUser> Process(StringReader text, ParseResult r)
        {
            var token = text.GetNextToken();

            if (ulong.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out var uid))
            {
                var result = await discord.Client.GetUserAsync(uid).ConfigureAwait(false);

                if (result == null)
                {
                    throw new Exception($"Invalid user '{token}'");
                }

                text.PopNextToken();
                return result;
            }

            var m = UserRegex.Match(token);
            if (m.Success && ulong.TryParse(m.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out uid))
            {
                var result = await discord.Client.GetUserAsync(uid).ConfigureAwait(false);
                if (result == null)
                {
                    throw new Exception($"Invalid user '{token}'");
                }
                
                text.PopNextToken();
                return result;
            }

            var cs = config.CaseSensitive;
            if (!cs)
                token = token.ToLowerInvariant();

            var di = token.IndexOf('#');
            var un = di != -1 ? token.Substring(0, di) : token;
            var dv = di != -1 ? token.Substring(di + 1) : null;

            var us = discord.Client.Guilds.Values
                .SelectMany(xkvp => xkvp.Members.Values)
                .Where(xm => (cs ? xm.Username : xm.Username.ToLowerInvariant()) == un && ((dv != null && xm.Discriminator == dv) || dv == null));

            var usr = us.FirstOrDefault();
            

            if(usr == null)
            {
                throw new Exception($"Invalid user '{token}'");
            }
            text.PopNextToken();
            return usr;

            throw new Exception($"Invalid user '{token}'");
        }

        public override string ToString()
        {
                return "DiscordUser()";   
        }

        //public override IEnumerable<string> Examples => UserExamples;

    }
    
}
