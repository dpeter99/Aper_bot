using System;
using System.Threading;
using System.Threading.Tasks;
using Aper_bot.Database;
using Aper_bot.EventBus;
using Aper_bot.Events;
using Aper_bot.Util;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ILogger = Serilog.ILogger;

namespace Aper_bot.Modules.Discord
{
    public class DiscordBot : Singleton<DiscordBot>, IHostedService
    {

        public DiscordClient Client { get; private set; }
        //public DiscordSlashClient SlashClient { get; private set; }

        ILogger<DiscordBot> Log;
        readonly IEventBus eventBus;

        readonly IDbContextFactory<DatabaseContext> dbContextFactory;

        public DiscordBot(ILogger<DiscordBot> logger, IEventBus bus, IOptions<Config> configuration, IDbContextFactory<DatabaseContext> fac, ILoggerFactory loggerFactory)
        {

            Log = logger;
            eventBus = bus;
            dbContextFactory = fac;

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
                logger.LogDebug("asd");
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
            var new_event = new MessageCreatedEvent(messageCreateArgs,dbContextFactory.CreateDbContext());

            _ = eventBus.PostEventAsync(new_event);

            return Task.CompletedTask;
        }



        public DiscordEmbedBuilder BaseEmbed()
        {
            return new DiscordEmbedBuilder()
            {
                Author = new DiscordEmbedBuilder.EmbedAuthor() {Name = "Aper_bot"},
                Timestamp = DateTimeOffset.Now
            };
        }
    }
}
