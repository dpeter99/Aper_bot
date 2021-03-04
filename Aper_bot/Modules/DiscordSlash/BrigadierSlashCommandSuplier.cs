using System.Collections.Generic;
using System.Linq;
using Aper_bot.Modules.CommandProcessing;
using Aper_bot.Modules.CommandProcessing.Commands;
using Aper_bot.Modules.DiscordSlash.Entities;
using Aper_bot.Util;
using Brigadier.NET.Tree;
using Microsoft.Extensions.Logging;

namespace Aper_bot.Modules.DiscordSlash
{
    class BrigadierSlashCommandSuplier : ISlashCommandSuplier
    {
        private ILogger<BrigadierSlashCommandSuplier> Logger { get; }
        private ICommandTree CommandTree { get; }

        public BrigadierSlashCommandSuplier(ILogger<BrigadierSlashCommandSuplier> logger, ICommandTree commandTree)
        {
            Logger = logger;
            CommandTree = commandTree;
        }

        public IEnumerable<SlashCommand> GetCommands()
        {
            var commands = new List<SlashCommand>();


            var dispatcher = CommandTree.dispatcher;

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


        SlashCommand? GetCommand(CommandNode<CommandExecutionContext> cmdNode)
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

            foreach (var subnode in cmdNode.OfType<LiteralCommandNode<CommandExecutionContext>>())
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


            var t = new SlashCommand();
            t._applicationCommand = command;
            return t;
        }

        ApplicationCommandOption? getOptions(LiteralCommandNode<CommandExecutionContext> node)
        {
            ApplicationCommandOption options = new ApplicationCommandOption();
            options.Name = node.Name;
            options.Description = !string.IsNullOrWhiteSpace(node.Description) ? node.Description : "No desc";

            if (node.Command != null ||
                IsEndNode(node))
            {
                options.Type = ApplicationCommandOptionType.SubCommand;

                var arguments = new List<ApplicationCommandOption>();
                
                var cmdParams = node.Map(n => n.Children).ToList();
                if (cmdParams.Count > 0 && cmdParams.All(n => n is ArgumentCommandNode<CommandExecutionContext>))
                {
                    foreach (var param in cmdParams.Select(commandNode => commandNode as ArgumentCommandNode<CommandExecutionContext>))
                    {
                        if(param is null)
                            continue;
                        
                        var argument = new ApplicationCommandOption();
                        argument.Name = param.Name;
                        argument.Description = !string.IsNullOrWhiteSpace(param.Description) ? param.Description : "No desc";
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
                
                foreach (var sub in node.OfType<LiteralCommandNode<CommandExecutionContext>>())
                {
                    subCmds.Add(getOptions(sub));
                }

                options.Options = subCmds.ToArray();
            }

            return options;
        }

        bool IsEndNode(LiteralCommandNode<CommandExecutionContext> node)
        {
            var descendants = node.Map(n => n.Children);

            return descendants.All(node => node is ArgumentCommandNode<CommandExecutionContext>);
        }
    }
}