using System.IO;
using System.Threading.Tasks;
using Aper_bot.Modules.Discord;
using DSharpPlus.SlashCommands.Entities.Builders;
using DSharpPlus.SlashCommands.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Aper_bot.Modules.DiscordSlash.Controllers
{
    [Route("api/discordslash")]
    [ApiController]
    public class DiscordSlashCommandController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly SlashCommandHandler _slashModule;

        public DiscordSlashCommandController(ILogger<DiscordSlashCommandController> logger, SlashCommandHandler slashModule)
        {
            _logger = logger;
            _slashModule = slashModule;
        }
        
        [HttpPost("")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        [ServiceFilter(typeof(DiscordValidationFilter))]
        public async Task<IActionResult> DiscordEndpointHandler()
        {
            using var reader = new StreamReader(Request.Body);
            if (reader.BaseStream.CanSeek)
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
            var raw = await reader.ReadToEndAsync();
            
            _logger.LogDebug(raw);
            
            // Response parsing
            JObject json;
            try
            { // ... attempt to create a json object from the body ...
                json = JObject.Parse(raw);
            }
            catch
            { // ... if that fails, return a 400 Bad Request.
                return BadRequest();
            }
            
            // ... check to see if this is a ping to the webhook ...
            if (json.ContainsKey("type") && (int)(json["type"] ?? "0") == (int)InteractionType.Ping)
            {
                return Ok(
                    JsonConvert.SerializeObject(
                        new InteractionResponseBuilder()
                            .WithType(InteractionResponseType.Pong)
                            .Build()
                    )
                ); // ... and return the pong if it is.
            }
            else
            {// ... then pass the raw request body to the client ...
                
                var response = await _slashModule.HandleWebhookPost(raw);
                if (response is not null) // ... if the clients response is not null ...
                    return Ok(JsonConvert.SerializeObject(response)); // ... serialize it and send it.
                
                else return BadRequest("Failed to parse request JSON."); // ... or send a bad request message.
                
            }
            //return BadRequest("Failed to parse request JSON.");
        }
    }
}