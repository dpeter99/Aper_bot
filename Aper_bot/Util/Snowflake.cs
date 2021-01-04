using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aper_bot.Util
{
    struct Snowflake
    {
        ulong id;

        public Snowflake(ulong i)
        {
            id = i;
        }

        public static implicit operator ulong(Snowflake d) => d.id;
        public static implicit operator Snowflake(ulong d) => new Snowflake(d);
    }
}
