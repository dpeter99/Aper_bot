using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Aper_bot.Database;
using Aper_bot.Database.Model;
using Aper_bot.Events;
using Aper_bot.Modules.CommandProcessing;
using Aper_bot.Modules.Discord;
using Aper_bot.Modules.Discord.Config;
using Aper_bot.Modules.DiscordSlash.Database;
using Aper_bot.Modules.DiscordSlash.Database.Model;
using Aper_bot.Modules.DiscordSlash.Entities;
using Aper_bot.Util;
using Aper_bot.Util.Discord;
using DSharpPlus.SlashCommands.Entities;
using Extensions.Hosting.AsyncInitialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using ApplicationCommand = Aper_bot.Modules.DiscordSlash.Entities.ApplicationCommand;
using ApplicationCommandOption = Aper_bot.Modules.DiscordSlash.Entities.ApplicationCommandOption;

namespace Aper_bot.Modules.DiscordSlash
{
    public class SlashCommandUpdater : IHostedService
    {
        private readonly ILogger<SlashCommandUpdater> _logger;

        private readonly IHttpClientFactory _httpClientFactory;

        private DiscordBot _bot;

        private ISlashCommandSuplier _commandSuplier;

        private readonly HttpClient _client;
        private readonly SlashDbContext _db;

        private string? token;
        private string? botID;

        private ConcurrentDictionary<Snowflake, SlashCommand> _commands = new();

        public SlashCommandUpdater(
            ILogger<SlashCommandUpdater> logger,
            IHttpClientFactory httpClientFactory,
            DiscordBot bot,
            ISlashCommandSuplier commandSuplier,
            IOptions<DiscordConfig> configuration,
            HttpClient http,
            SlashDbContext db
        )
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _bot = bot;

            token = configuration.Value.DiscordBotKey;
            botID = configuration.Value.BotID;

            _commandSuplier = commandSuplier;

            _client = http;
            _db = db;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            InitializeAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            //throw new NotImplementedException();
        }

        public async Task InitializeAsync()
        {
            await LoadCommandTree();
        }

        private async Task LoadCommandTree()
        {
            _logger.LogInformation("Starting command update to Discord");
            
            var suppliedCommands = _commandSuplier.GetCommands().ToList(); //Current commands

            //Load in the commands from discord
            var discordCmds = await GetCommands(null);

            if (suppliedCommands.Count<=0)
            {
                _logger.LogInformation("There are no commands registered");
            }

            await MergeAndUpdateCommands(suppliedCommands, discordCmds?.ToList() ?? new List<SlashCommand>());
        }

        private async Task MergeAndUpdateCommands(List<SlashCommand> registered, List<SlashCommand> discord)
        {
            
            List<SlashCommand> toUpdateDiscord = new();
            List<SlashCommand> toRemoveDiscord = new();
            
            var dbCommands = _db.Commands.Where(c => c.Guild == null);

            //Remove old commands from the database.
            
            foreach (var dbCommand in dbCommands)
            {
                var cmd = registered.FirstOrDefault(c => c.ApplicationCommand.Name == dbCommand.Name);
                if (cmd is null)
                {
                    _db.Commands.Remove(dbCommand);
                }
            }

            await _db.SaveChangesAsync();
            
            //Remove old commands form discord

            foreach (var discordCommand in discord)
            {
                var cmd = registered.FirstOrDefault(c => c.ApplicationCommand.Name == discordCommand.ApplicationCommand.Name);
                
                if (cmd is null)
                {
                    toRemoveDiscord.Add(discordCommand);
                }
            }
            
            //Check and update the existing commands to discord and DB

            foreach (SlashCommand command in registered)
            {
                var dbCommand = _db.Commands.FirstOrDefault(c => c.Name == command.ApplicationCommand.Name);
                if (dbCommand is null)
                {
                    toUpdateDiscord.Add(command);
                    continue;
                }
                
                var discordCommand = discord.FirstOrDefault(c => c.ApplicationCommand.Name == command.ApplicationCommand.Name);

                if (discordCommand is not null)
                {

                    
                    if (command.Meta!._version != dbCommand.Version)
                    {
                        
                        if (command.Meta._version > dbCommand.Version)
                        {
                            toUpdateDiscord.Add(command);
                            dbCommand!.Version = command.Meta._version;

                            _logger.LogInformation("{Name} is registered for update from:{VersionOld} to: {VersionNew}",
                                command.ApplicationCommand.Name,dbCommand!.Version,command.Meta._version);

                        }
                        else
                        {
                            _logger.LogError(
                                "Possible downgrade command version for: {Name}. Old version:{VersionOld} New version: {VersionNew}",
                                command.ApplicationCommand.Name,dbCommand!.Version,command.Meta._version);
                        }
                    }
                    else
                    {
                        _logger.LogInformation("Last version of {Name} is: {Version} no need to update",command.ApplicationCommand.Name,dbCommand!.Version);
                    }
                }
                else
                {
                    toUpdateDiscord.Add(command);
                    _logger.LogInformation("Discord doesn't have {Name} is: {Version} no need to update", command.ApplicationCommand.Name,dbCommand!.Version);
                }
            }
            
            
            
            //Remove / Update
            // ... then update/add the commands ...
            await UpdateOrAddCommands(toUpdateDiscord);
            // ... and delete any old commands ...
            await RemoveOldCommands(toRemoveDiscord);
            
        }

        private async Task<IEnumerable<SlashCommand>> GetCommands(Snowflake? guildId)
        {
            HttpRequestMessage msg = new();
            msg.Headers.Authorization = new("Bot", token);
            msg.Method = HttpMethod.Get;

            if (guildId is not null)
            {
                msg.RequestUri = new Uri($"https://discord.com/api/applications/{botID}/guilds/{guildId}/commands");
            }
            else
            {
                msg.RequestUri = new Uri($"https://discord.com/api/applications/{botID}/commands");
            }

            var response = await _client.SendAsync(msg);

            if (response.IsSuccessStatusCode)
            {
                var jsonResult = await response.Content.ReadAsStringAsync();

                var newCmds = JsonConvert.DeserializeObject<ApplicationCommand[]>(jsonResult);
                if (newCmds is not null)
                {
                    foreach (var command in newCmds)
                    {
                        _logger.LogInformation($"Discord has {command.Name}");
                    }
                }
                
                var newCommands = newCmds.Select(c => new SlashCommand(c, guildId, null));

                return newCommands;
            }
            else
            {
                // ... otherwise log the error.
                _logger.LogError(await response.Content.ReadAsStringAsync(), "Error while updating command");
            }

            return null;
        }


        private async Task UpdateOrAddCommands(IEnumerable<SlashCommand> toUpdate)
        {
            

            foreach (var command in toUpdate)
            {
                var guildId = command.Guild;
                
                using (var s = _logger.BeginScope(toUpdate))
                {
                    _logger.LogInformation("[    ] Updating command: {Comand}", command.ApplicationCommand.Name);

                    HttpRequestMessage msg = new();
                    msg.Headers.Authorization = new("Bot", token);
                    msg.Method = HttpMethod.Post;

                    var json = JsonConvert.SerializeObject(command.ApplicationCommand, Formatting.Indented, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        DefaultValueHandling = DefaultValueHandling.Ignore
                    });
                    msg.Content = new StringContent(json);
                    msg.Content.Headers.ContentType = new("application/json");


                    if (guildId is not null)
                    {
                        msg.RequestUri = new Uri($"https://discord.com/api/applications/{botID}/guilds/{guildId}/commands");
                    }
                    else
                    {
                        msg.RequestUri = new Uri($"https://discord.com/api/applications/{botID}/commands");
                    }

                    _logger.LogDebug("[    ] Sending message: {Json}", json);

                    var response = await _client.SendAsync(msg);

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResult = await response.Content.ReadAsStringAsync();

                        var newCommand = JsonConvert.DeserializeObject<ApplicationCommand>(jsonResult);

                        var slashCommand = new SlashCommand(newCommand, guildId, null);


                        var old = _db.Commands.SingleOrDefault(c => c.Name == newCommand.Name);
                        if (old is not null)
                        {
                            old.CommandID = newCommand.Id.ToString();
                        }
                        else
                        {
                            old = new Command(newCommand.Name, newCommand.Id.ToString(), command.Meta?._version ?? 0);
                            _db.Commands.Add(old);
                        }

                        await _db.SaveChangesAsync();

                        _commands.TryAdd(slashCommand.ApplicationCommand.Id, slashCommand);


                        _logger.LogInformation("[DONE] Command updated");
                        _logger.LogDebug("[   ] Response was: {Json}", jsonResult);
                    }
                    else
                    {
                        // ... otherwise log the error.
                        _logger.LogError(await response.Content.ReadAsStringAsync(), "Error while updating command Sent data:{data}",json);
                        
                    }
                }
            }
        }

        private async Task RemoveOldCommands(List<SlashCommand> toRemove)
        {
            foreach (var scfg in toRemove)
            {
                _logger.LogInformation("[    ] Removing command: {Comand}", scfg.ApplicationCommand.Name);
                // ... build a new HTTP request message ...
                HttpRequestMessage msg = new();
                msg.Headers.Authorization = new("Bot", token);
                msg.Method = HttpMethod.Delete;

                if (scfg.Guild is not null)
                {
                    msg.RequestUri = new Uri($"https://discord.com/api/applications/{botID}/guilds/{scfg.Guild}/commands/{scfg.ApplicationCommand.Id}");
                }
                else
                {
                    msg.RequestUri = new Uri($"https://discord.com/api/applications/{botID}/commands/{scfg.ApplicationCommand.Id}");
                }

                var response = await _client.SendAsync(msg);

                if (response.IsSuccessStatusCode)
                {
                    //Commands.TryRemove(scfg.Name, out _);
                    _logger.LogInformation("[DONE] Removing command: {Comand}", scfg.ApplicationCommand.Name);
                }
                else
                {
                    _logger.LogError($"Failed to delete command: ${response.ReasonPhrase}");
                }
            }
        }


        public void RunCommand()
        {
        }
    }

    public class SlashCommand
    {
        public ApplicationCommand ApplicationCommand;

        public CommandMetaData Meta;

        public Snowflake? Guild;

        public SlashCommand(ApplicationCommand applicationCommand, Snowflake? guild, CommandMetaData? meta)
        {
            ApplicationCommand = applicationCommand;
            this.Guild = guild;
            this.Meta = meta ?? new CommandMetaData(0);
        }
        
    }

    public class CommandArguments
    {
        public Dictionary<string, string> args = new();
    }
}