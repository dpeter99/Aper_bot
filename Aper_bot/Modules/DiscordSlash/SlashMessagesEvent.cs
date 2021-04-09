using System;
using System.Threading.Tasks;
using Aper_bot.Database;
using Aper_bot.Database.Model;
using Aper_bot.Events;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands.Entities;
using DSharpPlus.SlashCommands.Enums;

namespace Aper_bot.Modules.DiscordSlash
{
    public class SlashMessageEvent : CommandSourceStack<Entities.Interaction>, IMessageCreatedEvent
    {
        public string Token { get; }

        public CoreDatabaseContext Db => db;
        public Guild? Guild { get; }
        public User Author { get; }
        public string Message { get; }
        public DateTime Time { get; }



        public string Text;

        public DiscordEmbed? Embed;
        
        public SlashMessageEvent(Entities.Interaction args, CoreDatabaseContext database) : base(args, database)
        {
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