﻿using System;
using System.Buffers;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Aper_bot.Modules.Discord;
using Aper_bot.Modules.DiscordSlash.Entities;
using Aper_bot.Modules.DiscordSlash.Entities.Builders;
using DSharpPlus.SlashCommands.Entities.Builders;
using DSharpPlus.SlashCommands.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Aper_bot.Modules.DiscordSlash.Controllers
{
    /// <summary>
    /// This webhook is used by Discord to communicate the 
    /// </summary>
    [Route("api/discordslash")]
    [ApiController]
    [ApiVersionNeutral]
    public class DiscordSlashCommandController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly SlashCommandExecutor _slashModule;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="slashModule"></param>
        public DiscordSlashCommandController(ILogger<DiscordSlashCommandController> logger, SlashCommandExecutor slashModule)
        {
            _logger = logger;
            _slashModule = slashModule;
        }
        
        /// <summary>
        /// This endpoint handles the incoming discord slash command interactions. It uses the request body for the data, but it needs to be validated.
        /// </summary>
        /// <remarks>
        /// This endpoint should only be used by Discord for delivering interaction data.
        /// The data in the body needs to be signed by discord to work.
        /// </remarks>
        /// <response code="401">The signature couldn't be validated</response>
        /// <response code="200">Returns the responses for the interaction</response>
        /// <returns>The response to the interaction</returns>
        [HttpPost]
        [ServiceFilter(typeof(DiscordValidationFilter))]
        [Produces("application/json")]
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
                if (response is not null)
                {
                    var opt = new JsonSerializerSettings();
                    opt.DefaultValueHandling = DefaultValueHandling.Ignore;
                    
                    var j = JsonConvert.SerializeObject(response, opt);

                    _logger.LogInformation(j);
                    Console.WriteLine(j);
                    
                    var r = new JsonResult(response, opt);
                    r.ContentType = "application/json";
                    r.StatusCode = 200;

                    return r;
                    
                    
                }

                else return BadRequest("Failed to parse request JSON."); // ... or send a bad request message.
                
            }
            //return BadRequest("Failed to parse request JSON.");
        }
    }
}