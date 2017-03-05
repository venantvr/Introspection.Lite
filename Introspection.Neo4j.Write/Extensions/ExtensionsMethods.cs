using System;
using System.Collections.Generic;

namespace Introspection.Neo4j.Write.Extensions
{
    public static class ExtensionsMethods
    {
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> @do)
        {
            foreach (var item in items)
            {
                @do.Invoke(item);
            }
        }
    }
}