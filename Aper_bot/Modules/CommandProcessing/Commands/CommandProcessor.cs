using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aper_bot.Database;
using Aper_bot.EventBus;
using Aper_bot.Events;
using Aper_bot.Util.Singleton;
using Extensions.Hosting.AsyncInitialization;
using Mars;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aper_bot.Modules.CommandProcessing.Commands
{
    /// <summary>
    /// This class is going to handle the incoming commands and than forward them to the right place
    /// </summary>
    public class CommandProcessor : Singleton<CommandProcessor>, ICommandProcessor, IAsyncInitializer
    {
        
        public Mars.CommandTree tree { get; private set; } = new Mars.CommandTree();
        
        IEventBus eventBus;
        private readonly IServiceProvider _provider;
        ILogger<CommandProcessor> _logger;

        IDbContextFactory<CoreDatabaseContext> dbContextFactory;

        private readonly IEnumerable<ChatCommands> _commands;
        private readonly IOptions<CommandBaseConfig> _config;

        public CommandProcessor(IEventBus bus, IServiceProvider provider, ILogger<CommandProcessor> log, IDbContextFactory<CoreDatabaseContext> fac, IEnumerable<ChatCommands> commands,
            IOptions<CommandBaseConfig> config)
        {
            eventBus = bus;
            _provider = provider;
            _logger = log;
            dbContextFactory = fac;
            _commands = commands;
            _config = config;
            

        }

        public Task InitializeAsync()
        {
            eventBus.Register(this);

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
                "Found Commands:\n" +
                "{Modules}",
                String.Join("\n", _commands.Select(m => m.GetType().Name)));

            return Task.CompletedTask;
        }

        public void RunCommand(CommandExecutionContext ctx)
        {
            throw new NotImplementedException();
        }

        

        public void ProcessMessage(IMessageCreatedEvent messageEvent)
        {
            if (messageEvent.Message.Length == 0 || !messageEvent.Message.StartsWith(_config.Value.prefix))
            {
                return;
            }

            var commandText = messageEvent.Message.Remove(0, _config.Value.prefix.Length);

            
            var res = tree.ParseString(commandText);

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
                //messageEvent.Respond("WELL well");
                if (res.Callback is CommandFunction func)
                {
                    func.Cmd(res, messageEvent);
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