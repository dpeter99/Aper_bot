using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Aper_bot.Database;
using Aper_bot.Database.Model;
using Aper_bot.Hosting.WebHost;
using Aper_bot.Modules.Discord;
using Aper_bot.Modules.WebAPI.V2.Model;
using Aper_bot.Util.Discord;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Aper_bot.Modules.WebAPI.V2.Controllers
{
    /// <summary>
    /// This is responsible for retreating the quests from the bot for 3rd party usage
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("2.0")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(QuotesData), StatusCodes.Status200OK)]
    public class QuotesController : Controller
    {
        private readonly CoreDatabaseContext _db;
        private readonly DiscordBot _bot;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        public QuotesController(CoreDatabaseContext db, DiscordBot bot)
        {
            _db = db;
            _bot = bot;
        }

        /// <summary>
        /// Gives back the quests for a corresponding discord guild
        /// </summary>
        /// <remarks>
        /// Currently this endpoints gives back all the quotes form the requested server.
        /// In future versions of the API this will be limited by bot authentication as well as paging support.
        /// Changes:
        ///  - V2
        ///    - This version gives more data back
        /// </remarks>
        /// <param name="guildId">The ID of the guild you need the quests from</param>
        /// <returns></returns>
        /// <response code="404">The requested guild is not known by the bot</response>
        /// <response code="200">Returns a json containing all the quotes form the server</response>
        [Route("list/{guildId}")]
        [HttpGet]
        public async Task<IActionResult> Index(string guildId)
        {
            Guild g = _db.GetGuildFor(guildId);
            if (g is null)
            {
                return new NotFoundResult();
            }

            var dGuildAsync = _bot.Client.GetGuildAsync(g.GuildID);
            
            var quotes = from q in _db.Quotes.Include(q=>q.Source)
                where q.GuildID == g!.ID
                select q;

            List<QuoteData> quotesList = new();
            Dictionary<string,UserData> userList = new();
            Dictionary<string,RoleData> rolesList = new();

            foreach (var quote in quotes)
            {
                quotesList.Add(new QuoteData(quote.Text, quote.SourceName));
                if (quote.Source is not null)
                {
                    var user = quote.Source!;
                    if (!userList.ContainsKey(quote.SourceName))
                    {
                        var dGuild = await dGuildAsync;
                        var dUser = await dGuild.GetMemberAsync(new Snowflake(user.UserID));

                        var roles = (from r in dUser.Roles
                            select r.Name.Replace(' ','_')).ToList();
                        
                        userList.Add(quote.SourceName, new UserData(user.UserID, user.Name, dUser.AvatarUrl, Int32.Parse(dUser.Discriminator), roles));

                        foreach (var role in dUser.Roles)
                        {
                            if (!rolesList.ContainsKey(role.Name.Replace(' ','_')))
                            {
                                rolesList.Add(role.Name.Replace(' ','_'),new RoleData(role.Color.ToString(),role.Name));
                            }
                        }
                    }
                }
            }

            var res = new QuotesData()
            {
                Quotes = quotesList.ToList(),
                Users = userList,
                Roles = rolesList
            };
            
            return new JsonResult(res);
        }
    }
}