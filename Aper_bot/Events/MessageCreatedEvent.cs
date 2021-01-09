using Aper_bot.Database.Model;
using Aper_bot.Modules.Commands;

using DSharpPlus.EventArgs;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace Aper_bot.Events
{
    public class MessageCreatedEvent : CommandSourceStack<MessageCreateEventArgs>
    {

        public Guild? guild { get; private set; }

        public User author { get; private set; }

        public string Message { get; private set; }

        public MessageCreatedEvent(MessageCreateEventArgs args, Database.DatabaseContext database) : base(args, database)
        {
            guild = db.GetGuildFor(@event.Guild);

            author = db.GetOrCreateUserFor(@event.Author);

            Message = args.Message.Content;
        }

        public async void RespondError(string text)
        {
            var builder = new DiscordEmbedBuilder();

            builder.Author = new DiscordEmbedBuilder.EmbedAuthor()
            {
                Name = "Aper_bot"
            };
            builder.Color = new Optional<DiscordColor>(new DiscordColor(255, 0, 0));
            builder.Timestamp = DateTimeOffset.Now;
            builder.Title = "Error";
            builder.Description = text;

            await @event.Message.RespondAsync(embed: builder.Build());
        }

        public async void Respond(string text)
        {
            var builder = new DiscordEmbedBuilder();

            builder.Author = new DiscordEmbedBuilder.EmbedAuthor()
            {
                Name = "Aper_bot"
            };
            builder.Color = new Optional<DiscordColor>(new DiscordColor(255, 0, 0));
            builder.Timestamp = DateTimeOffset.Now;
            
            builder.Description = text;

            await @event.Message.RespondAsync(embed: builder.Build());
        }
        
    }
}
