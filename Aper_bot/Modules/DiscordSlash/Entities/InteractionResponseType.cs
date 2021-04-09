using System;

namespace DSharpPlus.SlashCommands.Enums
{
    public enum InteractionResponseType
    {
        /// <summary>
        /// ACK a Ping
        /// </summary>
        Pong = 1,
        
        /// <summary>
        /// DEPRECATED ACK a command without sending a message, eating the user's input
        /// </summary>
        [Obsolete]
        Acknowledge = 2,
        
        /// <summary>
        /// DEPRECATED respond with a message, eating the user's input
        /// </summary>
        [Obsolete]
        ChannelMessage = 3,
        
        /// <summary>
        /// respond to an interaction with a message
        /// </summary>
        ChannelMessageWithSource = 4,
        
        /// <summary>
        /// ACK an interaction and edit to a response later, the user sees a loading state
        /// </summary>
        DeferredChannelMessageWithSource = 5
    }
}
