using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aper_bot.Modules.CommandProcessing.Commands;
using Extensions.Hosting.AsyncInitialization;
using Microsoft.Extensions.Logging;

namespace Aper_bot.Modules.CommandProcessing.CommandTree
{
    public class CommandGraph : ICommandGraph, IAsyncInitializer
    {
        public Mars.CommandTree tree { get; private set; } = new();

        ILogger<CommandGraph> _logger;
        private readonly IEnumerable<ChatCommands> _commands;

        public CommandGraph(ILogger<CommandGraph> logger, IEnumerable<ChatCommands> commands)
        {
            _logger = logger;
            _commands = commands;
        }

        public Task InitializeAsync()
        {
            foreach (var command in _commands)
            {
                var cmds = command.Register();
                if (cmds is not null)
                {
                    foreach (var cmd in cmds)
                    {
                        tree.AddNode(cmd);
                    }
                }
            }

            _logger.LogInformation(
                "Found Command classes:\n" +
                "{Modules}",
                String.Join("\n", _commands.Select(m => m.GetType().Name)));

            _logger.LogInformation(
                "Found Commands:\n" +
                "{Modules}",
                String.Join("\n", tree.GetRoot().Children.Select(m => m.GetType().Name)));

            return Task.CompletedTask;
        }
    }
}