using System.Collections.Generic;
using DSharpPlus.Entities;
using Newtonsoft.Json;

namespace Aper_bot.Modules.DiscordSlash.Entities
{
    public class InteractionApplicationCommandCallbackData
    {
        [JsonProperty("tts")]
        public bool? TextToSpeech { get; internal set; }
        [JsonProperty("content")]
        public string Content { get; internal set; }

        [JsonProperty("embeds")] 
        public DiscordEmbed[]? Embeds { get; internal set; } = new DiscordEmbed[0];
        
        [JsonProperty("allowed_mentions")]
        public IEnumerable<IMention>? AllowedMentions { get; internal set; }
        
        [JsonProperty("flags")]
        public int? Flags { get; internal set; }
    }
}
