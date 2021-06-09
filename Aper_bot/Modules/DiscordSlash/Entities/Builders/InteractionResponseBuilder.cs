using DSharpPlus.SlashCommands.Entities.Builders;

namespace Aper_bot.Modules.DiscordSlash.Entities.Builders
{
    public class InteractionResponseBuilder : IBuilder<InteractionResponse>
    {
        public InteractionResponseType Type { get; set; }
        public InteractionApplicationCommandCallbackDataBuilder? Data { get; set; }

        public InteractionResponseBuilder WithType(InteractionResponseType type)
        {
            Type = type;
            return this;
        }

        public InteractionResponseBuilder WithData(InteractionApplicationCommandCallbackDataBuilder data)
        {
            Data = data;
            return this;
        }

        public InteractionResponse Build()
        {
            return new InteractionResponse()
            { 
                Type = Type,
                Data = Data?.Build()
            };
        }
    }
}
