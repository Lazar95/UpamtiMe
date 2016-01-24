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
    }
}
