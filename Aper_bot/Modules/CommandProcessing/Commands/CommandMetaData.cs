using Aper_bot.Util.Discord;

namespace Aper_bot.Modules.CommandProcessing
{
    /// <summary>
    /// The metadata that is use to store the description and version inside the Mars command tree.
    /// </summary>
    public class CommandMetaData : Mars.CommandMetadata
    {
        public int _version;
        public string _description;

        public Snowflake? guild_id = null;

        public CommandMetaData(int version, string description = "")
        {
            this._version = version;
            this._description = description;
        }
    }
}