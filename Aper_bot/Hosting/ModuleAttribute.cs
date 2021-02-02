using System;

namespace Aper_bot.Hosting
{
    public class ModuleAttribute: Attribute
    {
        public string _name;

        public ModuleAttribute(string name)
        {
            _name = name;
        }
    }
}