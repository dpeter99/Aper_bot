﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Aper_bot.Database;
using Aper_bot.Database.Model;
using Aper_bot.EventBus;
using Aper_bot.Events;
using Aper_bot.Modules.CommandProcessing.Commands;
using Aper_bot.Modules.Discord.Config;
using Aper_bot.Util;
using Aper_bot.Util.Singleton;
using Certes.Acme.Resource;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ILogger = Serilog.ILogger;

namespace Aper_bot.Modules.Discord
{
    public class DiscordBotStarter : IHostedService
    {
        private readonly DiscordBot _bot;

        public DiscordBotStarter(DiscordBot bot)
        {
            _bot = bot;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _bot.Client.ConnectAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _bot.Client.DisconnectAsync();
        }
    }
    
    public class DiscordBot : Singleton<DiscordBot>
    {

        public DiscordClient Client { get; private set; }
        

        ILogger<DiscordBot> Log;
        private readonly IEventBus _eventBus;


        public IServiceProvider Services { get; }
        
        public DiscordBot(
            ILogger<DiscordBot> logger,
            IEventBus eventBus,
            IOptions<DiscordConfig> configuration,
            IServiceProvider services,
            ILoggerProvider loggerProvider)
        {

            Log = logger;
            _eventBus = eventBus;

            Services = services;

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddProvider(loggerProvider);
                builder.SetMinimumLevel(LogLevel.Critical);
                
            });

            DiscordConfiguration config = new DiscordConfiguration
            {
                Token = configuration.Value.DiscordBotKey,
                TokenType = TokenType.Bot,
                LoggerFactory = loggerFactory
            };

            Client = new DiscordClient(config);
            
            
            Client.MessageCreated += NewMessage;
            Client.UnknownEvent += (sender, args) =>
            {
                //logger.LogDebug("asd");
                args.Handled = true;

                return Task.CompletedTask; 
            };
            

            Log.LogInformation("DiscordClientConfigured");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            
            return Client.ConnectAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Client.DisconnectAsync();
        }

        Task NewMessage(DiscordClient discordClient,MessageCreateEventArgs messageCreateArgs)
        {
            using (var scope = Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CoreDatabaseContext>();

                if (messageCreateArgs.Guild != null && dbContext.GetGuildFor(messageCreateArgs.Guild.Id) is null)
                {
                    var discordGuild = messageCreateArgs.Guild;
                    
                    Log.LogInformation("Registering guild: {GuildID}",discordGuild.Id);

                    dbContext.Add(new Guild(discordGuild.Name, discordGuild.Id.ToString()));
                    dbContext.SaveChanges();
                }
                
                var new_event = new DiscordMessageCreatedEvent(messageCreateArgs,dbContext);

                //_commandExecutor.ProcessMessage(new_event);
                _eventBus.PostEvent(new_event);
                
                
            }
            
            return Task.CompletedTask;
        }



        public DiscordEmbedBuilder BaseEmbed()
        {
            return new DiscordEmbedBuilder()
            {
                Author = new DiscordEmbedBuilder.EmbedAuthor() {Name = "Aper_bot"},
                Timestamp = DateTimeOffset.Now,
            };
        }
    }
}
