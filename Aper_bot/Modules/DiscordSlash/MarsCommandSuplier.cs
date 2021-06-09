using System.Collections.Generic;
using System.Linq;
using Aper_bot.Modules.CommandProcessing;
using Aper_bot.Modules.CommandProcessing.Commands;
using Aper_bot.Modules.CommandProcessing.CommandTree;
using Aper_bot.Modules.DiscordSlash.Entities;
using Aper_bot.Util;
using DSharpPlus.Entities;
using Mars;
using Microsoft.Extensions.Logging;

namespace Aper_bot.Modules.DiscordSlash
{
    public class MarsCommandSuplier: ISlashCommandSuplier
    {
        
        private ILogger<MarsCommandSuplier> Logger { get; }
        private ICommandGraph CommandTree { get; }

        public MarsCommandSuplier(ILogger<MarsCommandSuplier> logger, ICommandGraph commandTree)
        {
            Logger = logger;
            CommandTree = commandTree;
        } 
        
        
        public IEnumerable<SlashCommand> GetCommands()
        {
            var root = CommandTree.tree.GetRoot();

            List<SlashCommand> commands = new();
            foreach (var childNode in root.GetChildNodes())
            {
                var cmd = GetCommand(childNode);
                if (cmd is not null)
                {
                    commands.Add(cmd);
                }
            }

            return commands;
        }


        SlashCommand? GetCommand(CommandNode cmdNode)
        {
            var command = new ApplicationCommand();
            command.Name = cmdNode.Name;
            command.Description = "top level";
            var options = new List<ApplicationCommandOption>();

            foreach (var node in cmdNode.GetChildNodes())
            {
                options.Add(GetOption(node));
            }

            command.Options = options.ToArray();
            
            var meta = cmdNode.Meta as CommandMetaData;
            var t = new SlashCommand(command, meta?.guild_id, meta);
            //t._applicationCommand = command;
            return t;
        }


        ApplicationCommandOption? GetOption(CommandNode node)
        {
            ApplicationCommandOption option = getOptionRep(node);

            var options = new List<ApplicationCommandOption>();

            //All the sub nodes are arguments, we need to flatten them
            var nodes = node.GetChildNodes().Map(n => n.GetChildNodes()).ToList();
            if (nodes.All(i => i is IArgumentNode))
            {
                options.AddRange(nodes.Select(getOptionRep));
            }
            else
            {
                foreach (var n in node.GetChildNodes())
                {
                    options.Add(GetOption(n));
                }    
            }

            option.Options = options.ToArray();
            
            return option;
        }
        
        ApplicationCommandOption getOptionRep(CommandNode node)
        {
            ApplicationCommandOption option = new ApplicationCommandOption();
            option.Name = node.Name;
            option.Description = "Test: " + node.Name;

            if (node is LiteralNode)
            {
                var nodes = node.GetChildNodes().Map(n => n.GetChildNodes()).ToList();
                if (node.EndNode || nodes.All(i => i is IArgumentNode))
                {
                    option.Type = ApplicationCommandOptionType.SubCommand;    
                }
                else
                {
                    option.Type = ApplicationCommandOptionType.SubCommandGroup;
                }
                
            }
            else if (node is ArgumentNode<DiscordUser> userArg)
            {
                option.Type = ApplicationCommandOptionType.User;
                option.Required = true;
            }
            else if(node is ArgumentNode<string> stringArg)
            {
                option.Type = ApplicationCommandOptionType.String;
                option.Required = true;
            }
            else if(node is ArgumentNode<int>)
            {
                option.Type = ApplicationCommandOptionType.Integer;
                option.Required = true;
            }
            
            return option;
        }
    }
}