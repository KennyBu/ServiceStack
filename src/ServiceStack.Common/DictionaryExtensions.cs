using System;
using System.Collections.Generic;
using ServiceStack.Common;

namespace ServiceStack
{
    public static class DictionaryExtensions
    {
        public static TValue GetValueOrDefault<TValue, TKey>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            return dictionary.ContainsKey(key) ? dictionary[key] : default(TValue);
        }

        public static void ForEach<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, Action<TKey, TValue> onEachFn)
        {
            foreach (var entry in dictionary)
            {
                onEachFn(entry.Key, entry.Value);
            }
        }

        public static bool EquivalentTo<K, V>(this IDictionary<K, V> thisMap, IDictionary<K, V> otherMap)
        {
            if (thisMap == null || otherMap == null) return thisMap == otherMap;
            if (thisMap.Count != otherMap.Count) return false;

            foreach (var entry in thisMap)
            {
                V otherValue;
                if (!otherMap.TryGetValue(entry.Key, out otherValue)) return false;
                if (!Equals(entry.Value, otherValue)) return false;
            }

            return true;
        }

        public static List<T> ConvertAll<T, K, V>(IDictionary<K, V> map, Func<K, V, T> createFn)
        {
            var list = new List<T>();
            map.Each((kvp) => list.Add(createFn(kvp.Key, kvp.Value)));
            return list;
        }

        public static V GetOrAdd<K, V>(this Dictionary<K, V> map, K key, Func<K, V> createFn)
        {
            //simulate ConcurrentDictionary.GetOrAdd
            lock (map)
            {
                V val;
                if (!map.TryGetValue(key, out val))
                    map[key] = val = createFn(key);

                return val;
            }
        }

        public static KeyValuePair<TKey, TValue> PairWith<TKey, TValue>(this TKey key, TValue value)
        {
            return new KeyValuePair<TKey, TValue>(key, value);
        }

#if !SILVERLIGHT && !MONOTOUCH && !XBOX

        public static Dictionary<string, string> ToDictionary(this System.Collections.Specialized.NameValueCollection nameValues)
        {
            if (nameValues == null) return new Dictionary<string, string>();

            var map = new Dictionary<string, string>();
            foreach (var key in nameValues.AllKeys)
            {
                if (key == null)
                {
                    //occurs when no value is specified, e.g. 'path/to/page?debug'
                    //throw new ArgumentNullException("key", "nameValues: " + nameValues);
                    continue;
                }

                var values = nameValues.GetValues(key);
                if (values != null && values.Length > 0)
                {
                    map[key] = values[0];
                }
            }
            return map;
        }

        public static System.Collections.Specialized.NameValueCollection ToNameValueCollection(this Dictionary<string, string> map)
        {
            if (map == null) return new System.Collections.Specialized.NameValueCollection();

            var nameValues = new System.Collections.Specialized.NameValueCollection();
            foreach (var item in map)
            {
                nameValues.Add(item.Key, item.Value);
            }
            return nameValues;
        }

#endif
    }
}