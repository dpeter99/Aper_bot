using Aper_bot.Database.Model;
using DSharpPlus.EventArgs;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace Aper_bot.Events
{
    public class DiscordMessageCreatedEvent : CommandSourceStack<MessageCreateEventArgs>, IMessageCreatedEvent
    {

        public Guild? Guild { get; private set; }

        public User Author { get; private set; }

        public string Message { get; private set; }

        public DateTime Time
        {
            get
            {
                return this.@event.Message.Timestamp.DateTime;
            }
        }

        public DiscordMessageCreatedEvent(MessageCreateEventArgs args, Database.CoreDatabaseContext database) : base(args, database)
        {
            Guild = db.GetGuildFor(@event.Guild);

            Author = db.GetOrCreateUserFor(@event.Author);

            Message = args.Message.Content;
        }

        public async Task RespondError(string text)
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

        public async Task Respond(string text)
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
        
        public async Task<DiscordMessage> Respond(DiscordEmbed text)
        {
            return await @event.Message.RespondAsync(embed: text);
        }
        
    }
}
