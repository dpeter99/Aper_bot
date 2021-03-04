using Brigadier.NET;

namespace Aper_bot.Util.Brigadier
{
    public static class StringReaderHelper
    {
        public static string ReadTillSpace(this IStringReader reader)
        {
            var start = reader.Cursor;
            while (reader.CanRead() && !char.IsWhiteSpace(reader.Peek()))
            {
                reader.Skip();
            }

            var number = reader.String.Substring(start, reader.Cursor - start);
            return number;
        }

    }
}
