using Aper_bot.Database;
using Aper_bot.EventBus;

using Brigadier.NET;
using Brigadier.NET.Exceptions;

using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aper_bot.Modules.Commands
{
    /// <summary>
    /// This class is going to handle the incoming commands and than forward them to the right place
    /// </summary>
    class CommandHandler : IHostedService
    {
        internal CommandDispatcher<CommandSourceStack> dispatcher = new CommandDispatcher<CommandSourceStack>();

        List<ChatCommands> Commands = new List<ChatCommands>();

        IEventBus eventBus;
        Serilog.ILogger logger;

        IDbContextFactory<DatabaseContext> dbContextFactory;

        public CommandHandler(IEventBus bus, IServiceProvider provider, Serilog.ILogger log, IDbContextFactory<DatabaseContext> fac)
        {
            eventBus = bus;
            logger = log;
            dbContextFactory = fac;

            var commands = Assembly.GetExecutingAssembly().DefinedTypes.Where(e => e.CustomAttributes.Any(a => a.AttributeType == typeof(CommandProviderAttribute)));
            foreach (var item in commands)
            {
                if (item.IsSubclassOf(typeof(ChatCommands)))
                {
                    ChatCommands command = (ChatCommands)ActivatorUtilities.CreateInstance(provider, item);
                    dispatcher.Register(command.Register);
                    Commands.Add(command);
                }
            }
        }



        public Task StartAsync(CancellationToken cancellationToken)
        {
            eventBus.Register(this);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }


        [EventListener]
        public void NewMessage(MessageCreateEventArgs message)
        {
            if (message.Message.Content.Length == 0 || message.Message.Content[0] != '/')
            {
                return;
            }

            var context = new CommandSourceStack(message, dbContextFactory);


            var res = dispatcher.Parse(message.Message.Content, context);
            try
            {
                if (res.Exceptions.Count == 0 && res.Context.Command != null)
                {
                    dispatcher.Execute(res);

                    if(context.exectutionTask != null)
                    {
                        context.exectutionTask.Wait();
                    }
                }
                else
                {
                    CommandSyntaxException exc = res.Exceptions.FirstOrDefault().Value;

                    CommandError(exc, res, message);
                }

            }
            catch (Exception e)
            {
                logger.Error(e, $"Error in parsing command: {message.Message.Content}");
                if (e is CommandSyntaxException error)
                {
                    CommandError(error, res, message);
                }

            }

        }


        private void CommandError(CommandSyntaxException exc, ParseResults<CommandSourceStack>? parse, MessageCreateEventArgs message)
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


            var builder = new DiscordEmbedBuilder()
            {
                Description = text
            };

            message.Message.RespondAsync(embed: builder.Build());
        }

    }
}
