using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aper_bot.Util
{
    class SingletonMultipleInstanceException: Exception
    {
        public SingletonMultipleInstanceException(Type type) 
            : base($"Singleton type: {type.Name} was instanciated more than once")
        {

        }
    }
}
