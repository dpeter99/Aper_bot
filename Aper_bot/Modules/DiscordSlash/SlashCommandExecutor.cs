using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Aper_bot.Database;
using Aper_bot.Modules.CommandProcessing;
using Aper_bot.Modules.CommandProcessing.Commands;
using Aper_bot.Modules.DiscordSlash.Entities;
using Aper_bot.Util;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands.Entities;
using Mars;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Aper_bot.Modules.DiscordSlash
{
    public class SlashCommandExecutor
    {
        private ICommandProcessor tree;

        readonly IDbContextFactory<CoreDatabaseContext> dbContextFactory;
        
        public SlashCommandExecutor(ICommandProcessor tree, IDbContextFactory<CoreDatabaseContext> fac)
        {
            this.tree = tree;
            this.dbContextFactory = fac;
        }


        public async Task<object?> HandleWebhookPost(string raw)
        {
            var i = JsonConvert.DeserializeObject<Interaction>(raw);

            var jobj = JObject.Parse(raw);
            DiscordUser? user = jobj["member"]?["user"]?.ToObject<DiscordUser>();
            // ... because we cant serialize direct to a DiscordMember, we are working around this
            // and using a DiscordUser instead. I would have to set the Lib as upstream to this before I
            // would be able to change this.
            i.User = user;
            
            
            var messageEvent = new SlashMessageEvent(i, dbContextFactory.CreateDbContext());

            
            var res = Parse(i.Data);

            if (res.Callback is not null)
            {
                //messageEvent.Respond("WELL well");
                if (res.Callback is CommandFunction func)
                {
                    func.Cmd(res, messageEvent);
                }
            }
            else
            {
                messageEvent.RespondError("Could not find the command");
            }
            
            return messageEvent.GetResponse();
        }

        ParseResult Parse(ApplicationCommandInteractionData? interactionData)
        {
            var root = tree.tree.GetRoot();

            var res = new ParseResult();

            ParseInteraction(root, interactionData, res);

            return res;
        }

        public static void ParseInteraction(CommandNode node, ApplicationCommandInteractionData data, ParseResult parseResult)
        {
            foreach (var child in node.Children)
            {
                if (child.Name == data.Name)
                {
                    //ParseOptions(child, data.Options, parseResult);
                    ParseNew(child,data.Options,parseResult);
                }
            }
        }

        public static void ParseOptions(CommandNode node, ApplicationCommandInteractionDataOption[] data, ParseResult parseResult)
        {
            var nodes = node.GetChildNodes().Map(n => n.GetChildNodes()).ToList();
            if (node.EndNode || nodes.All(i => i is IArgumentNode))
            {
                foreach (var option in data)
                {
                    foreach (var child in nodes)
                    {
                        if (child.Name == option.Name)
                        {
                            //This is a argument with a value
                            if (option.Value is not null)
                            {
                                string d = option.Value.ToString();
                                if (child.CanParse(d))
                                {
                                    child.ParseSingleToken(d, parseResult);
                                }
                            }
                            if (child.EndNode)
                            {
                                //The command string is empty and we are end node
                                parseResult.Callback = child.Callback;
                            }
                        }
                    }
                }
            }
            else if(!node.Children.Any())
            {
                if (node.EndNode)
                {
                    //The command string is empty and we are end node
                    parseResult.Callback = node.Callback;
                }
            }
            else
            {
                foreach (var option in data)
                {
                    foreach (var child in node.Children)
                    {
                        if (child.Name == option.Name)
                        {
                            //This is a argument with a value
                            if (option.Value is not null && option.Value is string d)
                            {
                                if (child.CanParse(d))
                                {
                                    child.ParseSingleToken(d, parseResult);
                                }
                            }

                            if (option.Options is not null)
                            {
                                ParseOptions(child, option.Options, parseResult);
                            }
                        }
                    }
                }
            }
        }

        public static void ParseNew(CommandNode node, ApplicationCommandInteractionDataOption[]? data, ParseResult parseResult)
        {
            //We are the node from Mars
            //We were called because we can parse the token
            
            
            //if we are an end node we set the execution data
            if (node.EndNode)
            {
                parseResult.Callback = node.Callback;
            }
            
            var flatChildren = node.Children.Map(n => n.Children).ToList();

            IEnumerable<CommandNode> nodes;
            //All the children are argument nodes so we need to parse them flat
            nodes = flatChildren.All(i => i is IArgumentNode) ? flatChildren : node.Children;

            if(data is null)
                return;
            
            foreach (var option in data)
            {
                foreach (var child in nodes)
                {
                    //If we match the name
                    if (option.Name == child.Name)
                    {
                        var content = option.Value?.ToString() ?? option.Name;
                        if (child.CanParse(content))
                        {
                            child.ParseSingleToken(content, parseResult);
                            if (option.Options is not null)
                            {
                                ParseNew(child, option.Options, parseResult);
                            }
                        }
                    }
                }
            }
            


        }
    }

    static class MarsHelper
    {
    }
}