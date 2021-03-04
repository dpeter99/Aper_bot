using System;

namespace Aper_bot.Util.Singleton
{
    class SingletonNotInstantiated: Exception
    {
        public SingletonNotInstantiated(Type type):base($"{type.Name} is not yet Instantiated")
        {

        }
    }
}
