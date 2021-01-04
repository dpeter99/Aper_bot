using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aper_bot.Util
{
    public static class NumberToName
    {
        public static string ToName(this int n)
        {
            return new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten"}[n];
        }
    }
}
