using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Base.CustomType
{
    [Serializable]
    public class SerializedDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField, HideInInspector]
        private List<TKey> m_keys = new List<TKey>();
        
        [SerializeField, HideInInspector]
        private List<TValue> m_values = new List<TValue>();
        public void OnAfterDeserialize()
        {
            this.Clear();
            for (int i = 0; i < this.m_keys.Count && i < this.m_values.Count; i++)
            {
                this[this.m_keys[i]] = this.m_values[i];
            }
        }

        public void OnBeforeSerialize()
        {
            this.m_keys.Clear();
            this.m_values.Clear();

            foreach (var item in this)
            {
                this.m_keys.Add(item.Key);
                this.m_values.Add(item.Value);
            }
        }
        
        public void CopyFrom(IDictionary<TKey, TValue> dict)
        {
            this.Clear();
            foreach (var kvp in dict)
            {
                this[kvp.Key] = kvp.Value;
            }
        }
        
        public static void SortByKey(SerializedDictionary<TKey, TValue> dictionary, Comparison<TKey> comparison)
        {
            List<TKey> copyKeys = dictionary.m_keys;
            copyKeys.Sort(comparison);

            Dictionary<TKey, TValue> newMap = new Dictionary<TKey, TValue>();
            for (int index = 0; index < copyKeys.Count; index++)
            {
                newMap.Add(copyKeys[index], dictionary[copyKeys[index]]);
            }
            dictionary.CopyFrom(newMap);
        }

        public static void SortByValue(SerializedDictionary<TKey, TValue> dictionary, Comparison<TValue> comparison)
        {
            KeyValuePair<TKey, TValue>[] values = dictionary.ToArray();
            Array.Sort(values, (pairA, pairB) =>
            {
                return comparison(pairA.Value, pairB.Value);
            });

            Dictionary<TKey, TValue> newMap = new Dictionary<TKey, TValue>();
            for (int index = 0; index < values.Length; index++)
            {
                KeyValuePair<TKey, TValue> pair = values[index];
                newMap.Add(pair.Key, pair.Value);
            }
            dictionary.CopyFrom(newMap);
        }
    }
}