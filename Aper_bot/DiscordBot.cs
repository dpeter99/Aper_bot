using Aper_bot.EventBus;

using DSharpPlus;
using DSharpPlus.Entities;

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
    class DiscordBot : IHostedService
    {
        DiscordClient client;

        ILogger Log;
        IEventBus eventBus;

        public DiscordBot(ILogger logger, IEventBus bus, IConfiguration configuration)
        {
            Log = logger;
            eventBus = bus;

            DiscordConfiguration config = new DiscordConfiguration
            {
                Token = configuration["DiscordBotKey"],
                TokenType = TokenType.Bot
            };

            client = new DiscordClient(config);

            client.MessageCreated += async (s) =>
            {
                if(!s.Author.IsBot)
                eventBus.PostEvent(s);
            };

            Log.Information("DiscordClientConfigured");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            
            return client.ConnectAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return client.DisconnectAsync();
        }
    }
}
