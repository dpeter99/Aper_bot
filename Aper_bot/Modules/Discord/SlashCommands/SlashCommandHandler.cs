using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Aper_bot.Modules.Discord.SlashCommands.Entities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Aper_bot.Modules.Discord.SlashCommands
{
    public class SlashCommandHandler: IHostedService
    {
        ILogger<SlashCommandHandler> Logger { get; }
        IHttpClientFactory HttpClientFactory { get; }
        DiscordBot Bot { get; }
        ISlashCommandSuplier CommandSuplier { get; }

        private HttpClient Client;
        
        private string? token;
        private string? botID;
        
        private ConcurrentDictionary<string, ApplicationCommand> Commands { get; set; }

        public SlashCommandHandler(
            ILogger<SlashCommandHandler> logger,
            IHttpClientFactory httpClientFactory,
            DiscordBot bot,
            ISlashCommandSuplier commandSuplier,
            IOptions<Config> configuration,
            HttpClient http)
        {
            Logger = logger;
            HttpClientFactory = httpClientFactory;
            Bot = bot;

            token = configuration.Value.DiscordBotKey;
            botID = configuration.Value.BotID;

            CommandSuplier = commandSuplier;

            Client = http;
            
            LoadCommandTree();
        }


        private void LoadCommandTree()
        {
             CommandSuplier.GetCommands();
        }


        public async Task StartAsync(CancellationToken cancellationToken)
        {
            //throw new System.NotImplementedException();

            var cmds = CommandSuplier.GetCommands();
            
            await UpdateOrAddCommands(cmds);
        }

        private async Task UpdateOrAddCommands(IEnumerable<ApplicationCommand> toUpdate)
        {
            // For every command
            foreach(var update in toUpdate)
            {
                await UpdateCommand(update, "611652646721421463");
            }
        }

        private async Task UpdateCommand(ApplicationCommand update, string? GuildId)
        {
            var cmd = update;

            HttpRequestMessage msg = new();
            msg.Headers.Authorization = new("Bot", token);
            msg.Method = HttpMethod.Post;
            // ... read the command object, ignoring default and null fields ...
            var json = JsonConvert.SerializeObject(cmd, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            });
            // ... set the content of the request ...
            msg.Content = new StringContent(json);
            msg.Content.Headers.ContentType = new("application/json");


            if (GuildId is not null)
            {
                msg.RequestUri = new Uri($"https://discord.com/api/applications/{botID}/guilds/{GuildId}/commands");
            }
            else
            {
                msg.RequestUri = new Uri($"https://discord.com/api/applications/{botID}/commands");
            }

            // ... then send and wait for a response ...
            var response = await Client.SendAsync(msg);
            // ... if the response is a success ...
            if (response.IsSuccessStatusCode)
            {
                // ... get the new command data ...
                var jsonResult = await response.Content.ReadAsStringAsync();

                var newCommand = JsonConvert.DeserializeObject<ApplicationCommand>(jsonResult);

                // ... and the old command data ...
                //var oldCommand = Commands[update.Name];
                // ... then update the old command with the new command.
                //if (newCommand is not null && oldCommand is not null)
                //{
                //    oldCommand.ApplicationId = newCommand.ApplicationId;
                //    oldCommand.CommandId = newCommand.Id;
                //}
            }
            else
            {
                // ... otherwise log the error.
                Logger.LogError(await response.Content.ReadAsStringAsync());
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            //throw new System.NotImplementedException();
            return Task.CompletedTask;
        }
    }
}