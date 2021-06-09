using System;
using System.Threading.Tasks;
using Aper_bot.Database;
using Aper_bot.Database.Model;
using Aper_bot.Events;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace Aper_bot.Modules.Discord
{
    public class DiscordMessageCreatedEvent : CommandSourceStack<MessageCreateEventArgs>, IMessageCreatedEvent
    {
        public CoreDatabaseContext Db => db;

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

        public bool HasRefMessage => @event.Message.ReferencedMessage is not null;
        public string? RefMessageText => @event.Message.ReferencedMessage.Content;
        
        public User? RefMessageAuthor { get; }
        public DateTime? RefMessageTime => @event.Message.ReferencedMessage.Timestamp.DateTime;

        public DiscordMessageCreatedEvent(MessageCreateEventArgs args, Database.CoreDatabaseContext database) : base(args, database)
        {
            Guild = db.GetGuildFor(@event.Guild);

            Author = db.GetOrCreateUserFor(@event.Author);

            Message = args.Message.Content;

            if (HasRefMessage)
            {
                RefMessageAuthor = db.GetOrCreateUserFor(@event.Message.ReferencedMessage.Author);
            }
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
            builder.Color = new Optional<DiscordColor>(new DiscordColor(0, 150, 0));
            builder.Timestamp = DateTimeOffset.Now;
            
            builder.Description = text;
            

            await @event.Message.RespondAsync(embed: builder.Build());
        }
        
        public async Task<DiscordMessage> Respond(DiscordEmbed text)
        {
            return await @event.Message.RespondAsync(embed: text);
        }

        public async Task<DiscordMessage> RespondBasic(string text)
        {
            return await @event.Message.RespondAsync(text);
        }
    }
}
