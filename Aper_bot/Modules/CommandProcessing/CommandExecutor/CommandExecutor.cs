using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Aper_bot.Database;
using Aper_bot.EventBus;
using Aper_bot.Events;
using Aper_bot.Modules.CommandProcessing.Attributes;
using Aper_bot.Modules.CommandProcessing.CommandTree;
using Aper_bot.Modules.DiscordSlash;
using Aper_bot.Util.Singleton;
using Extensions.Hosting.AsyncInitialization;
using Mars;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aper_bot.Modules.CommandProcessing.Commands
{
    /// <summary>
    /// This class handles executing commands and processing the command meta data.
    /// </summary>
    public class CommandExecutor : Singleton<CommandExecutor>, ICommandExecutor
    {
        IEventBus eventBus;
        private readonly ICommandGraph _cmdTree;
        private readonly IServiceProvider _provider;
        ILogger<CommandExecutor> _logger;

        private readonly IEnumerable<ChatCommands> _commands;
        private readonly IOptions<CommandBaseConfig> _config;

        public CommandExecutor(IEventBus bus, ICommandGraph cmdTree, IServiceProvider provider, ILogger<CommandExecutor> log,
            IOptions<CommandBaseConfig> config)
        {
            eventBus = bus;
            _cmdTree = cmdTree;
            _provider = provider;
            _logger = log;
            _config = config;
        }
        
        
        
        public async Task RunCommand(ParseResult result, IMessageCreatedEvent msgEvent)
        {
            var callback = (result.Callback as CommandFunction)!;
            
            var method = callback.cmdMeta;
            var permission = method.GetCustomAttributes<CommandAttribute>();
                //from a in 
                //where a.AttributeType.IsAssignableTo(typeof(ICommandConditionProvider))
                //select a;

            var commandConditionProviders = permission.ToList();
            
            _logger.LogInformation(
                "Command {CmdName} has the following conditions:\n"+
                "{Conditions}",
                "CMD name",
                String.Join("\n",commandConditionProviders.Select(p => p.GetType().Name)));
            
            bool check = true;
            foreach (var attribute in commandConditionProviders)
            {
                var con = ActivatorUtilities.CreateInstance(_provider, attribute.GetCondition()) as CommandCondition;

                check = check && await con!.CheckCondition(callback, msgEvent, attribute);
            }

            //msgEvent.Respond(check ? "Okay" : "NopNop");
            
            if (check)
            {
                try
                {
                    callback.Cmd.Invoke(result,msgEvent);
                }
                catch (Exception e)
                {
                    msgEvent.RespondError(e.Message);
                }
            }
            else
            {
                
                //msgEvent.RespondError("Couldn't authenticate");
            }
        }

        

        public void ProcessMessage(IMessageCreatedEvent messageEvent)
        {
            if (messageEvent.Message.Length == 0 || !messageEvent.Message.StartsWith(_config.Value.prefix))
            {
                return;
            }

            var commandText = messageEvent.Message.Remove(0, _config.Value.prefix.Length);

            
            var res = _cmdTree.tree.ParseString(commandText);

            if (res.error is not null)
            {
                 var usages = res.Nodes.Last().GetUsages(res);
                 string text = "Usage";
                 foreach (var item in usages)
                 {
                     text += $"\n {item}";
                 }

                 messageEvent.RespondError(text);
            }
            if (res.Callback is not null)
            {
                try
                {
                    if (res.Callback is CommandFunction func)
                    {
                        //func.Cmd(res, messageEvent);
                        RunCommand(res, messageEvent);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogCritical(e,"Critical Exception while running command: {Message}", messageEvent.Message);

                    messageEvent.RespondError("Error");
                }
            } 
            
        }
        
/*
        public async void ExecuteCommand(CommandExecutionContext context)
        {
            //We didn't get a command to run 
            if (context.command == null || context.ctx == null) return;

            var method = context.command.Method;
            var permission =
                from a in method.CustomAttributes
                where a.AttributeType == typeof(ICommandConditionProvider)
                select a;

            bool check = true;
            foreach (var attribute in permission.Cast<ICommandConditionProvider>())
            {
                var con = ActivatorUtilities.CreateInstance(_provider, attribute.GetCondition(context)) as CommandCondition;

                check = check && await con!.CheckCondition(context, attribute);
            }

            if (check)
            {
                try
                {
                    await context.command.Invoke(context.ctx, context.Event);
                }
                catch (Exception e)
                {
                    await context.Event.RespondError(e.Message);
                }
            }
        }

        private void CommandError(CommandSyntaxException exc, ParseResult parse, IMessageCreatedEvent discordMessage)
        {
            string text = exc?.InnerException?.Message ?? exc?.Message ?? "";

            if (parse != null)
            {
                var sug_task = dispatcher.GetCompletionSuggestions(parse, parse.Reader.TotalLength);
                sug_task.Wait();
                var suggestions = sug_task.Result.List;
                if (suggestions.Count > 0)
                {
                    text += "\n Suggestions";
                    foreach (var item in suggestions)
                    {
                        text += $"\n {item.Text}";
                    }
                }
                else
                {
                    if (parse.Context.Nodes.Count > 0)
                    {
                        var usage = dispatcher.GetAllUsage(parse.Context.Nodes.Last().Node, parse.Context.Source, false);
                        //var usage = dispatcher.GetSmartUsage(parse.Context.Nodes.Last().Node, parse.Context.Source);
                        if (usage.Length > 0)
                        {
                            text += "\n Usage";
                            foreach (var item in usage)
                            {
                                text += $"\n {item}";
                            }
                        }
                    }
                }
            }

            discordMessage.RespondError(text);
        }
        */

    }

}