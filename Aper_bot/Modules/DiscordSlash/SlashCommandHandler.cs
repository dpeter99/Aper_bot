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
using Aper_bot.Database.Model;
using Aper_bot.Events;
using Aper_bot.Modules.CommandProcessing;
using Aper_bot.Modules.Discord;
using Aper_bot.Modules.DiscordSlash.Database;
using Aper_bot.Modules.DiscordSlash.Database.Model;
using Aper_bot.Util;
using Aper_bot.Util.Discord;
using Brigadier.NET;
using Brigadier.NET.Context;
using DSharpPlus.SlashCommands.Entities;
using Extensions.Hosting.AsyncInitialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using ApplicationCommand = Aper_bot.Modules.DiscordSlash.Entities.ApplicationCommand;
using ApplicationCommandOption = Aper_bot.Modules.DiscordSlash.Entities.ApplicationCommandOption;

namespace Aper_bot.Modules.DiscordSlash
{
    public class SlashCommandHandler : IHostedService
    {
        private readonly ILogger<SlashCommandHandler> _logger;

        private readonly IHttpClientFactory _httpClientFactory;

        private DiscordBot _bot;

        private ISlashCommandSuplier _commandSuplier;

        private readonly HttpClient _client;
        private readonly IDbContextFactory<SlashDbContext> _dbContextFactory;

        private string? token;
        private string? botID;

        private ConcurrentDictionary<Snowflake, SlashCommand> _commands = new ConcurrentDictionary<Snowflake, SlashCommand>();

        public SlashCommandHandler(
            ILogger<SlashCommandHandler> logger,
            IHttpClientFactory httpClientFactory,
            DiscordBot bot,
            ISlashCommandSuplier commandSuplier,
            IOptions<Config> configuration,
            HttpClient http,
            IDbContextFactory<SlashDbContext> dbContextFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _bot = bot;

            token = configuration.Value.DiscordBotKey;
            botID = configuration.Value.BotID;

            _commandSuplier = commandSuplier;

            _client = http;
            _dbContextFactory = dbContextFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return InitializeAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task InitializeAsync()
        {
            await LoadCommandTree();

            //var cmds = _commandSuplier.GetCommands();

            //await UpdateOrAddCommands(cmds);
        }

        private async Task LoadCommandTree()
        {
            var suppliedCommands = _commandSuplier.GetCommands().ToList(); //Current commands

            //Load in the commands from discord
            var discordCmds = await GetCommands("611652646721421463");
            //Load in the database
            using (var db = _dbContextFactory.CreateDbContext())
            {
                foreach (var cmd in suppliedCommands)
                {
                    var c = db.Commands.SingleOrDefault(d => d.Name == cmd._applicationCommand.Name);

                    if (c is not null)
                    {
                        cmd._applicationCommand.Id = ulong.Parse(c.CommandID);
                    }
                }
            }

            //Find new/changed
            //Find removed
            // ... build our update and delete lists ...
            List<SlashCommand> toUpdate = new();
            List<SlashCommand> toRemove = new();
            
            foreach (var cmd in discordCmds)
            {
                // ... see if Commands contains the command name ...
                var match = suppliedCommands.FirstOrDefault(c => c._applicationCommand.Id == cmd._applicationCommand.Id);
                if (match is not null)
                {
                    // ... if it is there, and the version number is lower in the saved state ...
                    if (cmd._applicationCommand != match._applicationCommand)
                    {
                        // ... queue the command for an update.
                        toUpdate.Add(cmd);
                    }
                    else
                    {
                        // ... otherwise, udpate the slash command with the saved values.
                        //slashCommand.CommandId = cmd.CommandId;
                        //slashCommand.GuildId = cmd.GuildId;
                        //slashCommand.ApplicationId = BotId;
                    }
                }
                else
                {
                    // ... if its in the config but not in the code
                    // queue the command for deletion.
                    toRemove.Add(cmd);
                }
            }

            //Remove / Update
            // ... then update/add the commands ...
            await UpdateOrAddCommands(toUpdate);
            // ... and delete any old commands ...
            await RemoveOldCommands(toRemove);
        }

        private async Task<IEnumerable<SlashCommand>> GetCommands(string? guildId)
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

                foreach (var command in newCmds)
                {
                    _logger.LogInformation($"Discord has {command.Name}");
                }

                var newCommands = newCmds.Select(c => new SlashCommand(c, guildId));

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
            var GuildId = "611652646721421463";

            foreach (var command in toUpdate)
            {
                using (var s = _logger.BeginScope(toUpdate))
                {
                    _logger.LogInformation("[    ] Updating command: {Comand}", command._applicationCommand.Name);

                    HttpRequestMessage msg = new();
                    msg.Headers.Authorization = new("Bot", token);
                    msg.Method = HttpMethod.Post;

                    var json = JsonConvert.SerializeObject(command._applicationCommand, Formatting.Indented, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        DefaultValueHandling = DefaultValueHandling.Ignore
                    });
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

                   
                    var response = await _client.SendAsync(msg);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResult = await response.Content.ReadAsStringAsync();

                        var newCommand = JsonConvert.DeserializeObject<ApplicationCommand>(jsonResult);

                        var slashCommand = new SlashCommand(newCommand, GuildId);

                        using (var db = _dbContextFactory.CreateDbContext())
                        {
                            var old = db.Commands.SingleOrDefault(c => c.Name == newCommand.Name);
                            if (old is not null)
                            {
                                old.CommandID = newCommand.Id.ToString();
                            }
                            else
                            {
                                old = new Command(newCommand.Name, newCommand.Id.ToString(), 1);
                                db.Commands.Add(old);
                            }

                            await db.SaveChangesAsync();

                            _commands.TryAdd(slashCommand._applicationCommand.Id, slashCommand);
                        }

                        _logger.LogInformation("[DONE] Command updated");
                    }
                    else
                    {
                        // ... otherwise log the error.
                        _logger.LogError(await response.Content.ReadAsStringAsync(), "Error while updating command");
                    }
                }
            }
        }

        private async Task RemoveOldCommands(List<SlashCommand> toRemove)
        {
            foreach (var scfg in toRemove)
            {
                _logger.LogInformation("[    ] Removing command: {Comand}", scfg._applicationCommand.Name);
                // ... build a new HTTP request message ...
                HttpRequestMessage msg = new();
                msg.Headers.Authorization = new("Bot", token);
                msg.Method = HttpMethod.Delete;

                if (scfg._guild is not null)
                {
                    msg.RequestUri = new Uri($"https://discord.com/api/applications/{botID}/guilds/{scfg._guild}/commands/{scfg._applicationCommand.Id}");
                }
                else
                {
                    msg.RequestUri = new Uri($"https://discord.com/api/applications/{botID}/commands/{scfg._applicationCommand.Id}");
                }

                var response = await _client.SendAsync(msg);

                if (response.IsSuccessStatusCode)
                {
                    //Commands.TryRemove(scfg.Name, out _);
                    _logger.LogInformation("[DONE] Removing command: {Comand}", scfg._applicationCommand.Name);
                }
                else
                {
                    _logger.LogError($"Failed to delete command: ${response.ReasonPhrase}");
                }
            }
        }

        public async Task<object?> HandleWebhookPost(string raw)
        {
            var i = JsonConvert.DeserializeObject<Interaction>(raw);

            _commands[i.Data.Id].Execute(i.Data);

            return null;
        }
    }

    public class SlashCommand
    {
        public ApplicationCommand _applicationCommand;

        public Command<CommandContext<CommandArguments>> run;

        public Snowflake? _guild;

        public SlashCommand(ApplicationCommand applicationCommand, Snowflake? guild)
        {
            _applicationCommand = applicationCommand;
            this._guild = guild;
        }

        public void Execute(ApplicationCommandInteractionData interactionData)
        {
            CommandArguments arguments = new();

            if (interactionData.Options is not null &&
                _applicationCommand.Options is not null)
            {
                ParseOptions(interactionData.Options, _applicationCommand.Options, arguments);
            }
            
            
            
        }

        private void ParseOptions(ApplicationCommandInteractionDataOption[] data, ApplicationCommandOption[] option, CommandArguments commandArguments)
        {
            if (data.Length > 0)
            {
                if (data.Length == 1)
                {
                    var op = data[0];
                    var next = option.SingleOrDefault(o => o.Name == op.Name);
                    if (next is not null)
                    {
                        ParseOptions(op.Options, next.Options, commandArguments);
                    }
                }
                else
                {
                    if (option.Length == data.Length)
                    {
                        foreach (var commandOption in data)
                        {
                            commandOption.Value?.Let(o => { commandArguments.args.Add(commandOption.Name, o.ToString()); });
                        }
                    }
                }
            }

            Debugger.Break();
        }
    }

    public class CommandArguments
    {
        public Dictionary<string, string> args = new();
    }
}