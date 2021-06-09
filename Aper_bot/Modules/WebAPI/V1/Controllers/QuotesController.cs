using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Aper_bot.Database;
using Aper_bot.Database.Model;
using Aper_bot.Modules.WebAPI.V1.Model;
using Aper_bot.Util.Discord;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quote = Aper_bot.Modules.WebAPI.V1.Model.Quote;
using User = Aper_bot.Modules.WebAPI.V1.Model.User;

namespace Aper_bot.Modules.WebAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("1.1")]
    public class QuotesController : Controller
    {
        private readonly CoreDatabaseContext _db;

        public QuotesController(CoreDatabaseContext db)
        {
            _db = db;
        }

        [Route("list/{guildId}")]
        [HttpGet]
        public JsonResult Index(string guildId)
        {
            Guild g = _db.GetGuildFor(guildId);

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