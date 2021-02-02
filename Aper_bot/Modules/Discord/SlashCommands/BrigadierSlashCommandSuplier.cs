using System.Collections.Generic;
using System.Linq;
using Aper_bot.Modules.CommandProcessing;
using Aper_bot.Modules.Discord.SlashCommands.Entities;
using Aper_bot.Util;
using Brigadier.NET.Tree;
using Microsoft.Extensions.Logging;

namespace Aper_bot.Modules.Discord.SlashCommands
{
    class BrigadierSlashCommandSuplier : ISlashCommandSuplier
    {
        private ILogger<BrigadierSlashCommandSuplier> Logger { get; }
        private ICommandHandler CommandHandler { get; }

        public BrigadierSlashCommandSuplier(ILogger<BrigadierSlashCommandSuplier> logger, ICommandHandler commandHandler)
        {
            Logger = logger;
            CommandHandler = commandHandler;
        }

        public IEnumerable<ApplicationCommand> GetCommands()
        {
            var commands = new List<ApplicationCommand>();


            var dispatcher = CommandHandler.dispatcher;

            var root = dispatcher.GetRoot();

            //var dis = CommandHandler.dispatcher;
            foreach (var topLevel in root)
            {
                var cmd = GetCommand(topLevel);
                if (cmd is not null)
                {
                    commands.Add(cmd);
                }
            }

            return commands;
        }


        ApplicationCommand? GetCommand(CommandNode<CommandArguments> cmdNode)
        {
            var command = new ApplicationCommand();
            command.Name = cmdNode.Name.Remove(0, 1);
            command.Description = "top leve";
            var options = new List<ApplicationCommandOption>();
            //If all the nodes are literals we make them into subcommands
            // server
            //  |- setup 
            //  |- something
            //      |- asd
            //      |- fgh

            foreach (var subnode in cmdNode.OfType<LiteralCommandNode<CommandArguments>>())
            {
                //The node is not used we make a group out of it

                var suboptions = getOptions(subnode);
                if (suboptions is not null)
                    options.Add(suboptions);
            }


            //var command = new ApplicationCommand(name, 0, options.ToArray(), 0L);


            /*
            //Break if the top level command is used.... discord limit
            if (cmdNode.Any(node => node is LiteralCommandNode<CommandArguments>) &&
                cmdNode.Any(node => node is ArgumentCommandNode<CommandArguments>))
            {
                Logger.LogError("Stuff");
                return null;
            }

            if (cmdNode.Command != null &&
                cmdNode.Children.Count > 0)
            {
                Logger.LogError("Stuff");
                return null;
            }
            */

            command.Options = options.ToArray();

            return command;
        }

        ApplicationCommandOption? getOptions(LiteralCommandNode<CommandArguments> node)
        {
            ApplicationCommandOption options = new ApplicationCommandOption();
            options.Name = node.Name;
            options.Description = "asdasd";

            if (node.Command != null ||
                IsEndNode(node))
            {
                options.Type = ApplicationCommandOptionType.SubCommand;

                var arguments = new List<ApplicationCommandOption>();
                
                var cmdParams = node.Map(n => n.Children).ToList();
                if (cmdParams.Count > 0 && cmdParams.All(n => n is ArgumentCommandNode<CommandArguments>))
                {
                    foreach (var param in cmdParams.Select(commandNode => commandNode as ArgumentCommandNode<CommandArguments>))
                    {
                        var argument = new ApplicationCommandOption();
                        argument.Name = param.Name;
                        argument.Description = "asd";
                        argument.Required = true;
                        var type = param.GetType().GenericTypeArguments[1];

                        var optionType = ApplicationCommandOptionTypeExtensions.GetOptionType(type);
                        if (optionType != null)
                        {
                            argument.Type = optionType.Value;
                        }
                        
                        arguments.Add(argument);
                    }
                    
                }

                options.Options = arguments.ToArray();
            }
            else
            {
                options.Type = ApplicationCommandOptionType.SubCommandGroup;

                var subCmds = new List<ApplicationCommandOption>();
                
                foreach (var sub in node.OfType<LiteralCommandNode<CommandArguments>>())
                {
                    subCmds.Add(getOptions(sub));
                }

                options.Options = subCmds.ToArray();
            }

            return options;
        }

        bool IsEndNode(LiteralCommandNode<CommandArguments> node)
        {
            var descendants = node.Map(n => n.Children);

            return descendants.All(node => node is ArgumentCommandNode<CommandArguments>);
        }
    }
}