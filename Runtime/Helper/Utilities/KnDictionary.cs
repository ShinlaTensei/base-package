#region Header
// Date: 22/07/2023
// Created by: Huynh Phong Tran
// File name: KnDictionary.cs
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Base.Helper
{
    public class KnDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] TKey[]   m_keys;
        [SerializeField] TValue[] m_values;

        public KnDictionary()
        {
        }

        public KnDictionary(IDictionary<TKey, TValue> dict) : base(dict.Count)
        {
            foreach (var kvp in dict)
            {
                this[kvp.Key] = kvp.Value;
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

        public void OnAfterDeserialize()
        {
            if (m_keys != null && m_values != null && m_keys.Length == m_values.Length)
            {
                this.Clear();
                int n = m_keys.Length;
                for (int i = 0; i < n; ++i)
                {
                    this[m_keys[i]] = m_values[i];
                }

                m_keys   = null;
                m_values = null;
            }
        }

        public void OnBeforeSerialize()
        {
            int n = this.Count;
            m_keys   = new TKey[n];
            m_values = new TValue[n];

            int i = 0;
            foreach (var kvp in this)
            {
                m_keys[i]   = kvp.Key;
                m_values[i] = kvp.Value;
                ++i;
            }
        }

        public static void SortByKey(KnDictionary<TKey, TValue> dictionary, Comparison<TKey> comparison)
        {
            TKey[] copyKeys = dictionary.m_keys;
            Array.Sort(copyKeys, comparison);

            Dictionary<TKey, TValue> newMap = new Dictionary<TKey, TValue>();
            for (int index = 0; index < copyKeys.Length; index++)
            {
                newMap.Add(copyKeys[index], dictionary[copyKeys[index]]);
            }
            dictionary.CopyFrom(newMap);
        }

        public static void SortByValue(KnDictionary<TKey, TValue> dictionary, Comparison<TValue> comparison)
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