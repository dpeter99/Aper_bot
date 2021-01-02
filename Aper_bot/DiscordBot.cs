using Aper_bot.EventBus;
using Aper_bot.Util;

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
     class DiscordBot : Singleton<DiscordBot>, IHostedService
    {

        public DiscordClient Client { get; private set; }

        ILogger Log;
        IEventBus eventBus;

        public DiscordBot(ILogger logger, IEventBus bus, IConfiguration configuration)
        {

            Log = logger;
            eventBus = bus;

            DiscordConfiguration config = new DiscordConfiguration
            {
                Token = configuration["DiscordBotKey"],
                TokenType = TokenType.Bot,
                
            };

            Client = new DiscordClient(config);

            Client.MessageCreated += async (c,s) =>
            {
                if(!s.Author.IsBot)
                eventBus.PostEvent(s);
            };

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
    }
}
