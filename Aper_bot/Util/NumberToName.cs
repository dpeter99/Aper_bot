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
        
        public static int[] GetIntArray(int num)
        {
            List<int> listOfInts = new List<int>();
            if (num == 0)
            {
                listOfInts.Add(0);
                return listOfInts.ToArray();
            }
            
            while(num > 0)
            {
                listOfInts.Add(num % 10);
                num = num / 10;
            }
            listOfInts.Reverse();
            return listOfInts.ToArray();
        }
    }
}
