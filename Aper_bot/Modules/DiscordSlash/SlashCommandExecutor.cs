﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Aper_bot.Database;
using Aper_bot.Database.Model;
using Aper_bot.EventBus;
using Aper_bot.Modules.CommandProcessing;
using Aper_bot.Modules.CommandProcessing.Commands;
using Aper_bot.Modules.CommandProcessing.CommandTree;
using Aper_bot.Modules.Discord;
using Aper_bot.Modules.DiscordSlash.Entities;
using Aper_bot.Util;
using Aper_bot.Util.Discord;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands.Entities;
using Mars;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Aper_bot.Modules.DiscordSlash
{
    public class SlashCommandExecutor
    {
        private ICommandGraph _tree;
        private readonly ICommandExecutor _executor;
        private readonly IEventBus _eventBus;
        private readonly ILogger<SlashCommandExecutor> _logger;

        public SlashCommandExecutor(ICommandGraph tree, ICommandExecutor executor, IServiceProvider services, IEventBus eventBus, SlashCommandWebhooks webhooks, ILogger<SlashCommandExecutor> logger)
        {
            _tree = tree;
            _executor = executor;
            _eventBus = eventBus;
            _logger = logger;
            Services = services;
        }
        
        public IServiceProvider Services { get; }


        public async Task<object?> HandleWebhookPost(string raw)
        {
            var i = JsonConvert.DeserializeObject<Interaction>(raw);
            if (i is null)
                return null;

            var jobj = JObject.Parse(raw);
            DiscordUser? user = jobj["member"]?["user"]?.ToObject<DiscordUser>();
            // ... because we cant serialize direct to a DiscordMember, we are working around this
            // and using a DiscordUser instead. I would have to set the Lib as upstream to this before I
            // would be able to change this.
            i.User = user;

            //_ = RunAsyncCommand(i);//.ConfigureAwait(false);
            _ = Task.Run(() =>  RunAsyncCommand(i));
            
            var inter = new InteractionResponse();
            inter.Data = new InteractionApplicationCommandCallbackData();
            inter.Data.TextToSpeech = false;
            inter.Data.Flags = 0;
            
            {
                inter.Type = InteractionResponseType.DeferredChannelMessageWithSource;
            }

            return inter;

        }

        private async Task RunAsyncCommand(Interaction? i)
        {
            using (var scope = Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CoreDatabaseContext>();
                
                if (i?.GuildId != null && dbContext.GetGuildFor(i.GuildId) is null)
                { 
                    var discordGuild = await DiscordBot.Instance.Client.GetGuildAsync(i.GuildId);
                    
                     _logger.LogInformation("Registering guild: {GuildID}",discordGuild.Id);

                    dbContext.Add(new Guild(discordGuild.Name, discordGuild.Id.ToString()));
                    dbContext.SaveChanges();
                }
                
                var messageEvent = new SlashMessageEvent(i, dbContext);

                

                var res = Parse(i.Data);

                if (res.Callback is not null)
                {
                    if (res.Callback is CommandFunction func)
                    {
                        //func.Cmd(res, messageEvent);
                        await _executor.RunCommand(res, messageEvent);
                        //_eventBus.PostEventAsync()
                    }
                }
                else
                {
                    //await messageEvent.RespondError("Could not find the command");
                }
            }
        }


        ParseResult Parse(ApplicationCommandInteractionData? interactionData)
        {
            var root = _tree.tree.GetRoot();

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
                            if (option.Options is not null || true)
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