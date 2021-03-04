using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.Intrinsics.Arm;
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
    public class SlashCommandHandler : IAsyncInitializer
    {
        ILogger<SlashCommandHandler> Logger { get; }
        IHttpClientFactory HttpClientFactory { get; }
        DiscordBot Bot { get; }
        ISlashCommandSuplier CommandSuplier { get; }

        private HttpClient Client;
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
            Logger = logger;
            HttpClientFactory = httpClientFactory;
            Bot = bot;

            token = configuration.Value.DiscordBotKey;
            botID = configuration.Value.BotID;

            CommandSuplier = commandSuplier;

            Client = http;
            _dbContextFactory = dbContextFactory;

            LoadCommandTree();
        }

        public async Task InitializeAsync()
        {
            var cmds = CommandSuplier.GetCommands();

            await UpdateOrAddCommands(cmds);
        }

        private void LoadCommandTree()
        {
            var cmds = CommandSuplier.GetCommands();

            foreach (var cmd in cmds)
            {
                using (var db = _dbContextFactory.CreateDbContext())
                {
                    var c = db.Commands.SingleOrDefault(d => d.Name == cmd._applicationCommand.Name);
                    if (c?.CommandID != null)
                        _commands.TryAdd(c.CommandID, cmd);
                }
            }
        }

        private async Task UpdateOrAddCommands(IEnumerable<SlashCommand> toUpdate)
        {
            // For every command
            foreach (var update in toUpdate)
            {
                await UpdateCommand(update, "611652646721421463");
            }
        }

        private async Task UpdateCommand(SlashCommand update, string? GuildId)
        {
            var cmd = update;

            using (var s = Logger.BeginScope(cmd))
            {
                Logger.LogInformation("Updating command: {Comand}", cmd._applicationCommand.Name);

                HttpRequestMessage msg = new();
                msg.Headers.Authorization = new("Bot", token);
                msg.Method = HttpMethod.Post;
                // ... read the command object, ignoring default and null fields ...
                var json = JsonConvert.SerializeObject(cmd._applicationCommand, Formatting.Indented, new JsonSerializerSettings
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


                    using (var db = _dbContextFactory.CreateDbContext())
                    {
                        var old = db.Commands.SingleOrDefault(c => c.Name == newCommand.Name);
                        if (old is not null)
                        {
                            old.CommandID = newCommand.Id.ToString();
                        }
                        else
                        {
                            db.Commands.Add(new Command(newCommand.Name, newCommand.Id.ToString(), 1));
                        }

                        await db.SaveChangesAsync();
                    }

                    // ... and the old command data ...
                    //var oldCommand = Commands[update.Name];
                    // ... then update the old command with the new command.
                    //if (newCommand is not null && oldCommand is not null)
                    //{
                    //    oldCommand.ApplicationId = newCommand.ApplicationId;
                    //    oldCommand.CommandId = newCommand.Id;
                    //}
                    Logger.LogInformation("Command {Comand}, updated", cmd._applicationCommand.Name);
                }
                else
                {
                    // ... otherwise log the error.
                    Logger.LogError(await response.Content.ReadAsStringAsync(), "Error while updating command");
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
                            commandOption.Value?.Let(o =>
                            {
                                commandArguments.args.Add(commandOption.Name, o.ToString());
                            });

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