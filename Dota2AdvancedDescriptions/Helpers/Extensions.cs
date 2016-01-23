﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dota2AdvancedDescriptions.Helpers
{
    public static class Extensions
    {
        public static IEnumerable<int> AllIndexesOf(this string str, string searchstring)
        {
            int minIndex = str.IndexOf(searchstring);
            while (minIndex != -1)
            {
                yield return minIndex;
                minIndex = str.IndexOf(searchstring, minIndex + searchstring.Length);
            }
        }

        public static int GoodIndex(this string str, string searchstring, int maxCount)
        {
            if (maxCount == 0) return str.IndexOf(searchstring);
            var indexes = str.AllIndexesOf(searchstring);
            var index = indexes.Count() <= maxCount ? indexes.ElementAt(0) : indexes.ElementAt(maxCount);
            return index;
        }
    }
}
