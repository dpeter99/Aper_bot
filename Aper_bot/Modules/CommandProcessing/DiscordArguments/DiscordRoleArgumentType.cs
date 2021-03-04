using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Aper_bot.Events;
using Aper_bot.Modules.Discord;
using Aper_bot.Util;
using Aper_bot.Util.Brigadier;
using Brigadier.NET;
using Brigadier.NET.Exceptions;
using DSharpPlus.Entities;
using Microsoft.Extensions.Options;

namespace Aper_bot.Modules.CommandProcessing.DiscordArguments
{
    class DiscordRoleArgumentType : AsyncArgument<DiscordRole>
    {
        private static readonly IEnumerable<string> UserExamples = new[] { "@username", "794664673487749131", "..." };

        private static Regex RoleRegex { get; }

        static DiscordRoleArgumentType()
        {
            RoleRegex = new Regex(@"^<@&(\d+?)>$", RegexOptions.ECMAScript | RegexOptions.Compiled);
        }


        Config config;

        DiscordBot discord;

        public DiscordRoleArgumentType(DiscordBot bot, IOptions<Config> conf)
        {
            config = conf.Value;
            discord = bot;
        }


        public override string ToString()
        {
                return "DiscordUser()";   
        }

        public async override Task<DiscordRole> Process<TSource>(IStringReader reader, TSource source)
        {
            var prew_cursor = reader.Cursor;

            string token = reader.ReadTillSpace();

            if (source is CommandExecutionContext arguments)
            {
                var ctx = ((DiscordMessageCreatedEvent)arguments.Event).@event;

                if (ctx.Guild == null)
                    throw new DynamicCommandExceptionType(value => new LiteralMessage($"Invalid role '{value}'")).CreateWithContext(reader, token);


                if (ulong.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out var rid))
                {
                    var result = ctx.Guild.GetRole(rid);
                    if(result == null)
                    {
                        throw new DynamicCommandExceptionType(value => new LiteralMessage($"Invalid role '{value}'")).CreateWithContext(reader, token);
                    }
                    return result;
                }

                var m = RoleRegex.Match(token);
                if (m.Success && ulong.TryParse(m.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out rid))
                {
                    var result = ctx.Guild.GetRole(rid);
                    if (result == null)
                    {
                        throw new DynamicCommandExceptionType(value => new LiteralMessage($"Invalid role '{value}'")).CreateWithContext(reader, token);
                    }
                    return result;
                }

                var cs = config.CaseSensitive;
                if (!cs)
                    token = token.ToLowerInvariant();

                var rol = ctx.Guild.Roles.Values.FirstOrDefault(xr => (cs ? xr.Name : xr.Name.ToLowerInvariant()) == token);
                if(rol==null)
                    throw new DynamicCommandExceptionType(value => new LiteralMessage($"Invalid role '{value}'")).CreateWithContext(reader, token);


                return rol;
            }
            
            throw new Exception($"This can only work with a {typeof(CommandExecutionContext)} TSource");
        }

        public override IEnumerable<string> Examples => UserExamples;
    }
}
