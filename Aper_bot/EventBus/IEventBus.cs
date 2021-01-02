using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aper_bot.EventBus
{
    interface IEventBus
    {
        public void Register(object listener);

        public void Unregister(object listener);

        public void PostEvent(object e);
    }
}
