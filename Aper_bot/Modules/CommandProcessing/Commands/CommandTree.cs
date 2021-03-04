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

namespace Aper_bot.Modules.CommandProcessing.Commands
{
    /// <summary>
    /// This class is going to handle the incoming commands and than forward them to the right place
    /// </summary>
    public class CommandTree : Singleton<CommandTree>, ICommandTree
    {
        public CommandDispatcher<CommandExecutionContext> dispatcher { get; private set; } = new CommandDispatcher<CommandExecutionContext>();

        List<ChatCommands> Commands = new List<ChatCommands>();

        IEventBus eventBus;
        private readonly IServiceProvider _provider;
        ILogger<CommandTree> _logger;

        IDbContextFactory<CoreDatabaseContext> dbContextFactory;

        public CommandTree(IEventBus bus, IServiceProvider provider, ILogger<CommandTree> log, IDbContextFactory<CoreDatabaseContext> fac, IEnumerable<ChatCommands> commands)
        {
            eventBus = bus;
            _provider = provider;
            _logger = log;
            dbContextFactory = fac;

            foreach (var command in commands)
            {
                dispatcher.Register(command.Register);
                Commands.Add(command);
            }
            
            _logger.LogInformation(
                "Found Commands:\n" +
                "{Modules}",
                String.Join("\n",Commands.Select(m => m.GetType().Name)));
        }

        //public async Task InitializeAsync()
        //{
            //eventBus.Register(this);
        //}

        public void RunCommand(CommandExecutionContext ctx)
        {
            throw new NotImplementedException();
        }
        
        public void ProcessMessage(IMessageCreatedEvent discordMessage)
        {
            if (discordMessage.Message.Length == 0 || discordMessage.Message[0] != '/')
            {
                return;
            }

            using (var db = dbContextFactory.CreateDbContext())
            {
                var context = new CommandExecutionContext(discordMessage);


                var res = dispatcher.Parse(discordMessage.Message, context);
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

                        CommandError(exc, res, discordMessage);
                    }

                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error in parsing command: {discordMessage.Message}");
                    if (e is CommandSyntaxException error)
                    {
                        CommandError(error, res, discordMessage);
                    }

                }

                db.SaveChanges();
            }
        }

        public async void ExecuteCommand(CommandExecutionContext context)
        {
            //We didn't get a command to run 
            if (context.command == null || context.ctx == null) return;

            using (var db = dbContextFactory.CreateDbContext())
            {
                context.db = db;
                
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
                        context.Event.RespondError(e.Message);
                    }

                }
            }

        }

        private void CommandError(CommandSyntaxException exc, ParseResults<CommandExecutionContext>? parse, IMessageCreatedEvent discordMessage)
        {
            string text = exc?.InnerException?.Message ?? exc?.Message ?? "";

            if (parse != null)
            {

                var sug_task = dispatcher.GetCompletionSuggestions(parse,parse.Reader.TotalLength);
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
