using System;
using System.Collections.Generic;
using Introspection.Analysis.Models.Introspection.Analysis.Models.Collections;

namespace Introspection.Analysis.Models.Introspection.Analysis.Models
{
    public static class ExtensionsMethods
    {
        public static void SafeAdd<T, TU>(this Dictionary<T, TU> dictionary, T key, TU value)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, value);
            }
        }

        public static void ForEach<T>(this BaseCollection<T> items, Action<T> @do)
        {
            foreach (var item in items)
            {
                @do.Invoke(item);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> items, Action<T> @do)
        {
            foreach (var item in items)
            {
                @do.Invoke(item);
            }
        }
    }
}