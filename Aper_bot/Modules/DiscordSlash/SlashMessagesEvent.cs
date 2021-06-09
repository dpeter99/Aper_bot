using System;
using System.Threading.Tasks;
using Aper_bot.Database;
using Aper_bot.Database.Model;
using Aper_bot.Events;
using Aper_bot.Modules.DiscordSlash.Entities;
using Aper_bot.Modules.DiscordSlash.Entities.Builders;
using Aper_bot.Util.Discord;
using DSharpPlus.Entities;


namespace Aper_bot.Modules.DiscordSlash
{
    public class SlashMessageEvent : CommandSourceStack<Interaction>, IMessageCreatedEvent
    {
        private readonly Interaction _args;
        public Snowflake InteractionID { get; }

        public string InteractionToken { get; }


        public CoreDatabaseContext Db => db;
        public Guild? Guild { get; }
        public User Author { get; }
        public string Message { get; }
        public DateTime Time
        {
            get
            {
                var ticks = (InteractionID >> 22) + 1420070400000;
                return DateTimeOffset.FromUnixTimeMilliseconds(ticks).DateTime;
            }
        }

        public bool HasRefMessage => false;
        public string? RefMessageText => null;
        public User? RefMessageAuthor => null;
        public DateTime? RefMessageTime  => null;


        public SlashMessageEvent(Interaction args, CoreDatabaseContext database) : base(args, database)
        {
            _args = args;
            InteractionID = args.Id;
            InteractionToken = args.Token;
            Guild = db.GetGuildFor(args.GuildId);

            Author = db.GetOrCreateUserFor(args.User);

        }

        public async Task RespondError(string text)
        {
            var builder = Discord.DiscordBot.Instance.BaseEmbed();
            builder.Description = text;
            builder.Color = new Optional<DiscordColor>(new DiscordColor(255, 0, 0));

            var embed = builder.Build();
            
            var resp = MakeResponse(text);

            await SlashCommandWebhooks.Instance.EditOriginalMessage(this, resp.Data);
            
            
        }

        public async Task Respond(string text)
        {
            var builder = Discord.DiscordBot.Instance.BaseEmbed();
            builder.Description = text;

            var embed = builder.Build();

            var resp = MakeResponse(text);

            await SlashCommandWebhooks.Instance.EditOriginalMessage(this, resp.Data);
            
        }

        public async Task<DiscordMessage> Respond(DiscordEmbed text)
        {
            var resp = MakeResponse(text);

            var m = await SlashCommandWebhooks.Instance.EditOriginalMessage(this, resp.Data);
            
            return m!;
        }

        public async Task<DiscordMessage> RespondBasic(string text)
        {
            var resp = MakeResponse(text);

            var m = await SlashCommandWebhooks.Instance.EditOriginalMessage(this, resp.Data);
            
            return m;
        }

        public InteractionResponse GetResponse()
        {
            var inter = new InteractionResponse();
            inter.Data = new InteractionApplicationCommandCallbackData();
            inter.Data.TextToSpeech = false;
            inter.Data.Flags = 0;
            
            {
                inter.Type = InteractionResponseType.DeferredChannelMessageWithSource;
            }
            
            return inter;
        }

        public InteractionResponse MakeResponse(string text)
        {
            var inter = new InteractionResponse();
            inter.Data = new InteractionApplicationCommandCallbackData();
            inter.Data.TextToSpeech = false;
            inter.Data.Flags = 0;
            
            inter.Type = InteractionResponseType.ChannelMessageWithSource;

            inter.Data.Content = text;
            
            return inter;
        }
        
        public InteractionResponse MakeResponse(DiscordEmbed text)
        {
            var inter = new InteractionResponse();
            inter.Data = new InteractionApplicationCommandCallbackData();
            inter.Data.TextToSpeech = false;
            inter.Data.Flags = 0;
            
            inter.Type = InteractionResponseType.ChannelMessageWithSource;

            inter.Data.Embeds = new []{text};
            
            return inter;
        }
    }
}