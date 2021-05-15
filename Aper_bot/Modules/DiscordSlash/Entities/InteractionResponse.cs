﻿using System;
using DSharpPlus.SlashCommands.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using InteractionResponseType = DSharpPlus.InteractionResponseType;

namespace Aper_bot.Modules.DiscordSlash.Entities
{
    public class InteractionResponse
    {
        [JsonProperty("type")]
        public InteractionResponseType Type { get; internal set; }
        
        [JsonProperty("data")]
        public InteractionApplicationCommandCallbackData? Data { get; internal set; }

        /// <summary>
        /// Builds the webhook body for sending a new message.
        /// </summary>
        /// <returns>Raw JSON body for a webhook POST operation.</returns>
        public string BuildWebhookBody(JsonSerializerSettings settings)
        {
            if (Data is null)
                throw new Exception("Data can not be null.");

            return JsonConvert.SerializeObject(Data, settings); 
        }

        /// <summary>
        /// Builds the webhook edit body for editing a previous message.
        /// </summary>
        /// <returns>Raw JSON body for a webhook PATCH operation.</returns>
        public string BuildWebhookEditBody(JsonSerializerSettings settings)
        {
            if (Data is null)
                throw new Exception("Data can not be null.");

            var d = JObject.Parse(JsonConvert.SerializeObject(Data, settings));

            if (d.ContainsKey("tts"))
                d.Remove("tts");

            return d.ToString();
        }
    }
}
