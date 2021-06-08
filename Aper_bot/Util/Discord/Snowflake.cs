using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Aper_bot.Util.Discord
{
    public struct Snowflake
    {
        ulong id;

        public Snowflake(ulong i)
        {
            id = i;
        }
        
        public Snowflake(string i)
        {
            id = ulong.Parse(i);
        }

        public static implicit operator ulong(Snowflake d) => d.id;
        public static implicit operator Snowflake(ulong d) => new Snowflake(d);
        
        public static implicit operator long(Snowflake d)
        {
            unchecked
            {
                return (long) d.id;
            }
        }
        
        public static implicit operator Snowflake(long d)
        {
            unchecked
            {
                return new Snowflake((ulong)d);    
            }
            
        }

        
        public static implicit operator Snowflake(string d) => new (ulong.Parse(d));
        

        public override string ToString()
        {
            return id.ToString();
        }

        public static ValueConverter GetConverter() => new ValueConverter<Snowflake, long>(
            v => v,
            v => v);

    }
}
