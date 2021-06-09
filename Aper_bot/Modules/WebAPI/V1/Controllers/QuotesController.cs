using System.Collections.Generic;
using System.Linq;
using System.Net;
using Aper_bot.Database;
using Aper_bot.Database.Model;
using Aper_bot.Modules.WebAPI.V1.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quote = Aper_bot.Modules.WebAPI.V1.Model.Quote;
using User = Aper_bot.Modules.WebAPI.V1.Model.User;

namespace Aper_bot.Modules.WebAPI.V1.Controllers
{
    /// <summary>
    /// This is responsible for retreating the quests from the bot for 3rd party usage
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    public class QuotesController : Controller
    {
        private readonly CoreDatabaseContext _db;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        public QuotesController(CoreDatabaseContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Gives back the quests for a corresponding discord guild
        /// </summary>
        /// <remarks>
        /// Currently this endpoints gives back all the quotes form the requested server.
        /// In future versions of the API this will be limited by bot authentication as well as paging support.
        /// </remarks>
        /// <param name="guildId">The ID of the guild you need the quests from</param>
        /// <returns></returns>
        /// <response code="404">The requested guild is not known by the bot</response>
        /// <response code="200">Returns a json containing all the quotes form the server</response>
        [Route("list/{guildId}")]
        [HttpGet]
        [Produces( "application/json" )]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(QuotesData), StatusCodes.Status200OK)]
        public IActionResult Index(string guildId)
        {
            Guild g = _db.GetGuildFor(guildId);
            if (g is null)
            {
                return new NotFoundResult();
            }
            
            
            var quotes = from q in _db.Quotes.Include(q=>q.Source)
                where q.GuildID == g!.ID
                select q;

            List<Quote> quotesList = new();
            Dictionary<string,User> userList = new();

            foreach (var quote in quotes)
            {
                quotesList.Add(new Quote(quote.Text, quote.SourceName));
                if (quote.Source is not null)
                {
                    var user = quote.Source!;

                    userList.TryAdd(quote.SourceName, new User(user.UserID,user.Name, ""));
                }
            }

            var res = new QuotesData()
            {
                Quotes = quotesList.ToList(),
                Users = userList
            };
            
            return new JsonResult(res);
        }
    }
}