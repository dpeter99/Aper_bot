using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aper_bot.Database;
using Aper_bot.EventBus;
using Aper_bot.Events;
using Aper_bot.Util.Singleton;
using Brigadier.NET;
using Brigadier.NET.Exceptions;
using Extensions.Hosting.AsyncInitialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aper_bot.Modules.CommandProcessing.Commands
{
    /// <summary>
    /// This class is going to handle the incoming commands and than forward them to the right place
    /// </summary>
    public class CommandTree : Singleton<CommandTree>, ICommandTree, IAsyncInitializer
    {
        public CommandDispatcher<CommandExecutionContext> dispatcher { get; private set; } = new CommandDispatcher<CommandExecutionContext>();

        //List<ChatCommands> Commands = new List<ChatCommands>();

        IEventBus eventBus;
        private readonly IServiceProvider _provider;
        ILogger<CommandTree> _logger;

        IDbContextFactory<CoreDatabaseContext> dbContextFactory;

        private readonly IEnumerable<ChatCommands> _commands;
        private readonly IOptions<CommandBaseConfig> _config;

        public CommandTree(IEventBus bus, IServiceProvider provider, ILogger<CommandTree> log, IDbContextFactory<CoreDatabaseContext> fac, IEnumerable<ChatCommands> commands,
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
                dispatcher.Register(command.Register);
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


            var context = new CommandExecutionContext(messageEvent);

            var res = dispatcher.Parse(commandText, context);
            try
            {
                if (res.Exceptions.Count == 0 && res.Context.Command != null)
                {
                    dispatcher.Execute(res);

                    ExecuteCommand(context);
                }
                else
                {
                    CommandSyntaxException exc = res.Exceptions.FirstOrDefault().Value;

                    CommandError(exc, res, messageEvent);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error in parsing command: {messageEvent.Message}");
                if (e is CommandSyntaxException error)
                {
                    CommandError(error, res, messageEvent);
                }
            }
        }

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

        private void CommandError(CommandSyntaxException exc, ParseResults<CommandExecutionContext>? parse, IMessageCreatedEvent discordMessage)
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
    }
}