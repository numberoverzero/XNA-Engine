﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Utility
{
    /// <summary>
    /// Uses TryGetValue for index-notation lookup
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class DefaultDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        // TODO Add comments

        public DefaultDictionary() : base() { }
        public DefaultDictionary(IDictionary<TKey, TValue> dictionary) :base(dictionary) { }
        public new TValue this[TKey key]
        {
            get
            {
                TValue value;
                base.TryGetValue(key, out value);
                return value;
            }
            set
            {
                base[key] = value;
            }
        }
    }

    /// <summary>
    /// Provides an iterable set of unique entries.
    /// Adding an item increases the count of that item by 1.
    /// Removing an item decreases the count by 1.
    /// When the count is zero, the value is removed from the set.
    /// 
    /// The iterator is over unique values
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CountedSet<T> : IEnumerable<T>
    {
        // TODO Add comments

        private HashSet<T> set;
        private DefaultDictionary<T, int> count;

        public CountedSet()
        {
            set = new HashSet<T>();
            count = new DefaultDictionary<T, int>();
        }

        public CountedSet(IEnumerable<T> enumerable) : this()
        {
            foreach (T item in enumerable)
                Add(item);
        }

        public CountedSet(CountedSet<T> countedSet)
        {
            set = new HashSet<T>(countedSet.set);
            count = new DefaultDictionary<T, int>(countedSet.count);
        }

        public void Add(T item)
        {
            if (count[item] <= 0)
                set.Add(item);
            count[item]++;
        }

        public void Clear()
        {
            set.Clear();
            count.Clear();
        }

        public bool Contains(T item)
        {
            return set.Contains(item);
        }

        public int Count
        {
            get { return set.Count; }
        }

        public bool Remove(T item)
        {
            if (set.Contains(item))
            {
                count[item]--;
                if (count[item] <= 0)
                    return set.Remove(item);
            }
            // If the set doesn't contain the item or there was more than one reference to the item, we didn't remove it.
            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return set.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    /// <summary>
    /// A DefaultDict that supports two keys.
    /// </summary>
    /// <typeparam name="TKey1"></typeparam>
    /// <typeparam name="TKey2"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class DefaultMultiKeyDict<TKey1, TKey2, TValue>
    {
        // TODO Add comments

        DefaultDictionary<TKey1, DefaultDictionary<TKey2, TValue>> dict;

        public DefaultMultiKeyDict()
        {
            dict = new DefaultDictionary<TKey1, DefaultDictionary<TKey2, TValue>>();
        }

        public DefaultMultiKeyDict(DefaultMultiKeyDict<TKey1, TKey2, TValue> defaultMultiKeyDict)
        {
            dict = new DefaultDictionary<TKey1, DefaultDictionary<TKey2, TValue>>(defaultMultiKeyDict.dict);
        }

        public void Clear()
        {
            dict.Clear();
        }

        public TValue this[TKey1 key1, TKey2 key2]
        {
            get
            {
                return dict[key1][key2];
            }
            set
            {
                dict[key1][key2] = value;
            }
        }

        public DefaultDictionary<TKey1, DefaultDictionary<TKey2, TValue>>.KeyCollection Keys
        {
            get
            {
                return dict.Keys;
            }
        }
    }
}