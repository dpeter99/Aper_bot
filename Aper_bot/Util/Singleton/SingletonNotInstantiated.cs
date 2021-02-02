using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aper_bot.Util
{
    class SingletonNotInstantiated: Exception
    {
        public SingletonNotInstantiated(Type type):base($"{type.Name} is not yet Instantiated")
        {

        }
    }
}
