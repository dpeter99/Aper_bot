using System;
using System.Net.Http;
using System.Threading.Tasks;
using Aper_bot.Modules.Discord.Config;
using Aper_bot.Modules.DiscordSlash.Entities;
using Aper_bot.Util.Singleton;
using DSharpPlus.Entities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Aper_bot.Modules.DiscordSlash
{
    public class SlashCommandWebhooks : Singleton<SlashCommandWebhooks>
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<DiscordConfig> _configuration;
        private JsonSerializerSettings? _jsonSerializerSettings;

        private string? token => _configuration.Value.DiscordBotKey;

        private string? botID => _configuration.Value.BotID;

        public SlashCommandWebhooks(HttpClient httpClient, IOptions<DiscordConfig> configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            
            _jsonSerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };
        }

        public async Task<DiscordMessage?> EditOriginalMessage(SlashMessageEvent messageEvent, InteractionApplicationCommandCallbackData? response)
        {
            HttpRequestMessage msg = new();
            msg.Headers.Authorization = new("Bot", token);
            msg.Method = HttpMethod.Patch;

            msg.RequestUri = new Uri($"https://discord.com/api/webhooks/{botID}/{messageEvent.InteractionToken}/messages/@original");
            
            var json = JsonConvert.SerializeObject(response, Formatting.Indented, _jsonSerializerSettings);
            msg.Content = new StringContent(json);
            msg.Content.Headers.ContentType = new("application/json");

            var result = await _httpClient.SendAsync(msg);
            
            if (result.IsSuccessStatusCode)
            {
                var jsonResult = await result.Content.ReadAsStringAsync();

                var newCommand = JsonConvert.DeserializeObject<DiscordMessage>(jsonResult);
                
                //_logger.LogInformation("[DONE] Command updated");
                //_logger.LogDebug("[   ] Response was: {Json}", jsonResult);

                return newCommand;
            }
            
            return null;
        }
    }
}