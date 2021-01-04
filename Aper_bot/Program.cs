using System;
using System.Threading.Tasks;

using Aper_bot.EventBus;
using Aper_bot.Modules;
using Aper_bot.Modules.Commands;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Serilog;
using Serilog.Events;

namespace Aper_bot
{
    static class Program
    {

        static void Main(string[] args)
        {
            var app = new Application(args);

            app.Run();
        }


    }
}
