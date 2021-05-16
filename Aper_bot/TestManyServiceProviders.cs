using System;
using System.Threading;
using System.Threading.Tasks;
using Aper_bot.Database;
using Aper_bot.Modules.Discord;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Aper_bot
{
    public class TestManyServiceProviders : IHostedService
    {
        private readonly ILogger<TestManyServiceProviders> _logger;

        public IServiceProvider Services { get; }

        public TestManyServiceProviders(IServiceProvider services, ILogger<TestManyServiceProviders> logger)
        {
            _logger = logger;
            Services = services;
            
            _logger.LogInformation("TestManyServiceProviders Created");
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Running tests");
            
            for (int i = 0; i < 100; i++)
            {
                _logger.LogInformation($"Running test {i}");
                
                using (var scope = Services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<CoreDatabaseContext>();

                    dbContext.GetGuildFor("611652646721421463");

                    //var new_event = new DiscordMessageCreatedEvent(,dbContext);


                }
                
                _logger.LogInformation($"Finished test {i}");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}