using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aper_bot.Modules.Discord;

namespace Aper_bot.Util
{
    public class Singleton<T> where T : Singleton<T>
    {
        static T? _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                    throw new SingletonNotInstantiated(typeof(Application));
                return _instance;
            }

            internal set
            {
                if (_instance != null)
                    throw new SingletonMultipleInstanceException(typeof(DiscordBot));
                _instance = value;
            }
        }

        public Singleton()
        {
            Instance = (T)this;
        }
    }
}
