using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aper_bot.EventBus
{
    public interface IEventBus
    {
        public void Register(object listener);

        public void Unregister(object listener);

        public void PostEvent(Event e);

        /// <summary>
        /// This executes the event listeners on a different thread.
        /// Probably shoudn't be awaited.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public Task PostEventAsync(Event e);
    }
}
