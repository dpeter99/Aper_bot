using System;
using System.Threading.Tasks;
using Aper_bot.Database;
using Aper_bot.Database.Model;
using Aper_bot.Events;
using Aper_bot.Modules.DiscordSlash.Entities;
using Aper_bot.Util.Discord;
using DSharpPlus.Entities;


namespace Aper_bot.Modules.DiscordSlash
{
    public class SlashMessageEvent : CommandSourceStack<Interaction>, IMessageCreatedEvent
    {
        public Snowflake InteractionID { get; }

        
        public CoreDatabaseContext Db => db;
        public Guild? Guild { get; }
        public User Author { get; }
        public string Message { get; }
        public DateTime Time { get; }



        public string Text;

        public DiscordEmbed? Embed;
        
        public SlashMessageEvent(Entities.Interaction args, CoreDatabaseContext database, Snowflake interactionId) : base(args, database)
        {
            InteractionID = interactionId;
            Guild = db.GetGuildFor(args.GuildId);

            Author = db.GetOrCreateUserFor(args.User);

            //Message = args.Message.Content;
        }
        
        public Task RespondError(string text)
        {
            //throw new NotImplementedException();
            var builder = Discord.DiscordBot.Instance.BaseEmbed();
            builder.Description = text;
            builder.Color = new Optional<DiscordColor>(new DiscordColor(255, 0, 0));
            
            Embed = builder.Build();
            //Text = text;
            return Task.CompletedTask;
        }

        public Task Respond(string text)
        {
            //throw new NotImplementedException();
            var builder = Discord.DiscordBot.Instance.BaseEmbed();
            builder.Description = text;

            Embed = builder.Build();
            
            return Task.CompletedTask;
        }

        public async Task<DiscordMessage> Respond(DiscordEmbed text)
        {
            //throw new NotImplementedException();
            this.Embed = text;
            return null;
        }

        public Task<DiscordMessage> RespondBasic(string text)
        {
           // throw new NotImplementedException();
           this.Text = text;
           return null;
        }

        public InteractionResponse GetResponse()
        {
            var inter = new InteractionResponse();
            inter.Data = new InteractionApplicationCommandCallbackData();
            inter.Data.TextToSpeech = false;
            inter.Data.Flags = 0;
            
            if (Embed is not null)
            {
                inter.Type = InteractionResponseType.ChannelMessageWithSource;
                inter.Data.Embeds = new[] {Embed};
            }
            else if (string.IsNullOrWhiteSpace(Text))
            {
                inter.Type = InteractionResponseType.DeferredChannelMessageWithSource;
                
            }
            else
            {
                inter.Type = InteractionResponseType.ChannelMessageWithSource;
                

                inter.Data.Content = Text;
            }

            return inter;
        }
    }
}