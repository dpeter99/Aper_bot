using Aper_bot.Util;

using Brigadier.NET;
using Brigadier.NET.ArgumentTypes;
using Brigadier.NET.Exceptions;

using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

using Serilog;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Aper_bot.Modules.Commands.DiscordArguments
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

            if (source is CommandArguments arguments)
            {
                var ctx = arguments.Event.@event;

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
            
            throw new Exception($"This can only work with a {typeof(CommandArguments)} TSource");
        }

        public override IEnumerable<string> Examples => UserExamples;
    }
}
