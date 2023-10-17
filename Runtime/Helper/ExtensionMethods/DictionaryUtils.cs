using System;
using System.Collections.Generic;

namespace Base.Helper
{
    public static class DictionaryUtils
    {
        public static void CopyFrom<TKey, TValue>(this IDictionary<TKey, TValue> thisDict, IDictionary<TKey, TValue> dict)
        {
            thisDict.Clear();
            foreach (var kvp in dict)
            {
                thisDict[kvp.Key] = kvp.Value;
            }
        }

        public static void AddFrom<TKey, TValue>(this IDictionary<TKey, TValue> thisDict, IDictionary<TKey, TValue> dict)
        {
            foreach (var kvp in dict)
            {
                thisDict[kvp.Key] = kvp.Value;
            }
        }

        public static void SortByKey<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Comparison<TKey> comparison)
        {
            List<TKey> copyKeys = new List<TKey>();
            IDictionary<TKey, TValue> newMap = new Dictionary<TKey, TValue>();

            foreach (TKey key in dictionary.Keys)
            {
                copyKeys.Add(key);
            }

            copyKeys.Sort(comparison);

            for (int index = 0; index < copyKeys.Count; index++)
            {
                newMap.Add(copyKeys[index], dictionary[copyKeys[index]]);
            }

            dictionary.CopyFrom(newMap);
        }

        public static void SortByValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Comparison<TValue> comparison)
        {
            List<KeyValuePair<TKey, TValue>> values = new List<KeyValuePair<TKey, TValue>>();
            foreach (KeyValuePair<TKey, TValue> pair in dictionary)
            {
                values.Add(pair);
            }

            values.Sort((pairA, pairB) =>
            {
                return comparison(pairA.Value, pairB.Value);
            });

            IDictionary<TKey, TValue> newMap = new Dictionary<TKey, TValue>();
            for (int index = 0; index < values.Count; index++)
            {
                KeyValuePair<TKey, TValue> pair = values[index];
                newMap.Add(pair.Key, pair.Value);
            }

            dictionary.CopyFrom(newMap);
        }

        public static TValue GetValue<TKey, TValue>(this IDictionary<TKey, TValue> mapper, TKey key)
        {
            if (mapper.TryGetValue(key, out TValue value))
            {
                return value;
            }

            return default;
        }
    }
}