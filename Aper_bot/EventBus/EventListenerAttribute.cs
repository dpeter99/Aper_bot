﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aper_bot.EventBus
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class EventListenerAttribute : Attribute
    {

    }
}
