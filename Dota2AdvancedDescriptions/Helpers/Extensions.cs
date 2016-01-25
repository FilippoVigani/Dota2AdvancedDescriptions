using System;
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

        public static int FirstIndexMatch<TItem>(this IEnumerable<TItem> items, Func<TItem, bool> matchCondition)
        {
            var index = 0;
            foreach (var item in items)
            {
                if (matchCondition.Invoke(item))
                {
                    return index;
                }
                index++;
            }
            return -1;
        }

        public static int LastIndexMatch<TItem>(this IEnumerable<TItem> items, Func<TItem, bool> matchCondition)
        {
            for(int i = items.Count()-1; i >= 0; i--)
            {
                if (matchCondition.Invoke(items.ElementAt(i)))
                {
                    return i;
                }
            }
            return -1;
        }

        public static V GetValueOrDefault<K, V>(this IDictionary<K, V> dic, K key)
        {
            return dic.ContainsKey(key) ? dic[key] : default(V);
        }
    }
}
