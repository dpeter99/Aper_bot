using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Aper_bot.EventBus
{
    public class EventBus : IEventBus
    {
        //public static EventBus Instance { get { return instance ?? (instance = new EventBus()); } }

        //private static EventBus instance;

        public EventBus() { }

        private List<EventListenerWrapper> listeners = new List<EventListenerWrapper>();

        public void Register(object listener)
        {
            if (!listeners.Any(l => l.Listener == listener))
                listeners.Add(new EventListenerWrapper(listener));
        }

        public void Unregister(object listener)
        {
            listeners.RemoveAll(l => l.Listener == listener);
        }

        public void PostEvent(Event e)
        {
            listeners.ForEach(l => l.PostEvent(e));

            //e.Dispose();
        }

        public async Task PostEventAsync(Event e)
        {
            await Task.Run(() =>
              {
                  PostEvent(e);
              });
        }


        private class EventListenerWrapper
        {
            public object Listener { get; private set; }
            //public Type EventType { get; private set; }

            //private MethodBase method;
            private Dictionary<Type, MethodBase> listeners = new Dictionary<Type, MethodBase>();

            public EventListenerWrapper(object listener)
            {
                Listener = listener;

                Type type = listener.GetType();

                var methodes = type.GetMethods().Where(e => e.CustomAttributes.Any(a => a.AttributeType == typeof(EventListenerAttribute)));

                foreach (var item in methodes)
                {
                    ParameterInfo[] parameters = item.GetParameters();
                    if (parameters.Length != 1)
                        throw new ArgumentException("Method OnEvent of class " + type.Name + " have invalid number of parameters (should be one)");

                    listeners.TryAdd(parameters[0].ParameterType, item);
                }

                //method = type.GetMethod("OnEvent");
                //if (method == null)
                //    throw new ArgumentException("Class " + type.Name + " does not containt method OnEvent");

                //ParameterInfo[] parameters = method.GetParameters();
                //if (parameters.Length != 1)
                //    throw new ArgumentException("Method OnEvent of class " + type.Name + " have invalid number of parameters (should be one)");

                //EventType = parameters[0].ParameterType;
            }

            public void PostEvent(object e)
            {
                listeners[e.GetType()].Invoke(Listener, new[] { e });
                //method.Invoke(Listener, new[] { e });
            }
        }


    }
}
