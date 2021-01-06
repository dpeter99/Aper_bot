using Aper_bot.Database;
using Aper_bot.EventBus;
using Aper_bot.Events;
using Aper_bot.Util;

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using Serilog;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aper_bot
{
     class DiscordBot : Singleton<DiscordBot>, IHostedService
    {

        public DiscordClient Client { get; private set; }

        ILogger Log;
        IEventBus eventBus;

        IDbContextFactory<DatabaseContext> dbContextFactory;

        public DiscordBot(ILogger logger, IEventBus bus, IConfiguration configuration, IDbContextFactory<DatabaseContext> fac)
        {

            Log = logger;
            eventBus = bus;
            dbContextFactory = fac;

            DiscordConfiguration config = new DiscordConfiguration
            {
                Token = configuration["DiscordBotKey"],
                TokenType = TokenType.Bot,
                
            };

            Client = new DiscordClient(config);

            Client.MessageCreated += NewMessage;

            Log.Information("DiscordClientConfigured");
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
            var new_event = new MessageCreatedEvent(messageCreateArgs,dbContextFactory.CreateDbContext());

            _ = eventBus.PostEventAsync(new_event);

            return Task.CompletedTask;
        }
    }
}
