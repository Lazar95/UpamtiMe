using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public static class Extentions
    {
        public static string removeExtraSeparator(this string s)
        {
            if (s[s.Length-1] == '|')
                s=s.Remove(s.Length - 1);
            return s;
        }

        public static string beautify(this float a)
        {
            if (a > 1000000)
            {
               return (a/ 1000000).ToString("0.00") + "m";
            }
            else if (a > 1000)
            {
                return (a / 1000).ToString("0.00") + "k";
            }
            else
            {
                return a.ToString("0.00");
            }
        }

        public static int ClosestTo(this IEnumerable<int> collection, int target)
        {
            // NB Method will return int.MaxValue for a sequence containing no elements.
            // Apply any defensive coding here as necessary.
            var closest = int.MaxValue;
            var minDifference = int.MaxValue;
            foreach (var element in collection)
            {
                var difference = Math.Abs((long)element - target);
                if (minDifference > difference)
                {
                    minDifference = (int)difference;
                    closest = element;
                }
            }

            return closest;
        }
    }
}
