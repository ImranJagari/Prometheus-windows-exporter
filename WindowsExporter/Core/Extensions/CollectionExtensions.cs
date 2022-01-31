using System;
using System.Collections.Generic;
using System.Linq;

namespace WindowsExporter.Core.Extensions
{
    public static class CollectionExtensions
    {
#if NETCOREAPP3_1
        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> items, Func<T, TKey> property)
        {
            return items.GroupBy(property).Select(x => x.First());
        }
#endif
    }
}
