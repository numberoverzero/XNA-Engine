using System;
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
        /// <summary>
        /// Construct an empty DefaultDictionary
        /// </summary>
        public DefaultDictionary() : base() { }
        
        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="dictionary"></param>
        public DefaultDictionary(IDictionary<TKey, TValue> dictionary) :base(dictionary) { }

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get or set.</param>
        /// <returns></returns>
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
        private HashSet<T> set;
        private DefaultDictionary<T, int> count;

        /// <summary>
        /// Construct an empty CountedSet
        /// </summary>
        public CountedSet()
        {
            set = new HashSet<T>();
            count = new DefaultDictionary<T, int>();
        }

        /// <summary>
        /// Copy the values of an enumerable into a CountedSet.
        /// Note that duplicates will increment the count, and are not included in the iterable set twice.
        /// </summary>
        /// <param name="enumerable"></param>
        public CountedSet(IEnumerable<T> enumerable) : this()
        {
            foreach (T item in enumerable)
                Add(item);
        }

        /// <summary>
        /// Copy the values of another counted set, as well as the count for each item.
        /// </summary>
        /// <param name="countedSet"></param>
        public CountedSet(CountedSet<T> countedSet)
        {
            set = new HashSet<T>(countedSet.set);
            count = new DefaultDictionary<T, int>(countedSet.count);
        }

        /// <summary>
        /// Add an item to the set.  If the item is already present, simply increments that item's count.
        /// </summary>
        /// <param name="item">The item to add</param>
        public void Add(T item)
        {
            if (count[item] <= 0)
                set.Add(item);
            count[item]++;
        }

        /// <summary>
        /// Clear the set and all associated counts.
        /// </summary>
        public void Clear()
        {
            set.Clear();
            count.Clear();
        }

        /// <summary>
        /// Checks if the specified item is present in the CountedSet.
        /// </summary>
        /// <param name="item">The item to look for.</param>
        /// <returns>True if the item's count is greater than 0.</returns>
        public bool Contains(T item)
        {
            return set.Contains(item);
        }

        /// <summary>
        /// Gets the number of elements that are contained in the set.
        /// </summary>
        public int Count
        {
            get { return set.Count; }
        }

        /// <summary>
        /// Remove an item from the set.  If the item is present, simply decrements that item's count.
        /// If the item's count is less than 1, the item is removed from the iterator.
        /// </summary>
        /// <param name="item">The item to remove</param>
        public bool Remove(T item)
        {
            if (set.Contains(item))
            {
                count[item]--;
                if (count[item] < 1)
                    return set.Remove(item);
            }
            // If the set doesn't contain the item or there was more than one reference to the item, we didn't remove it.
            return false;
        }

        /// <summary>
        /// Returns an enumerator that iterates through a Engine.Utility.CountedSet&lt;T&gt; object.
        /// </summary>
        /// <returns></returns>
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
    /// <typeparam name="TKey1">The first key type</typeparam>
    /// <typeparam name="TKey2">The second key type</typeparam>
    /// <typeparam name="TValue">The value type stored in the dictionary</typeparam>
    public class DefaultMultiKeyDict<TKey1, TKey2, TValue>
    {
        DefaultDictionary<TKey1, DefaultDictionary<TKey2, TValue>> dict;

        /// <summary>
        /// Construct an empty Double-keyed dictionary
        /// </summary>
        public DefaultMultiKeyDict()
        {
            dict = new DefaultDictionary<TKey1, DefaultDictionary<TKey2, TValue>>();
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="defaultMultiKeyDict"></param>
        public DefaultMultiKeyDict(DefaultMultiKeyDict<TKey1, TKey2, TValue> defaultMultiKeyDict)
        {
            dict = new DefaultDictionary<TKey1, DefaultDictionary<TKey2, TValue>>(defaultMultiKeyDict.dict);
        }

        /// <summary>
        /// Clear all values from the DefaultMultiKeyDict
        /// </summary>
        public void Clear()
        {
            dict.Clear();
        }

        /// <summary>
        /// Gets or sets the value associated with the specified keys.
        /// </summary>
        /// <param name="key1">The first key of the value to get or set.</param>
        /// <param name="key2">The second key of the value to get or set.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets a collection containing the keys in the Engine.Utility.DefaultMultiKeyDict&lt;TKey1, TKey2, TValue&gt;.
        /// </summary>
        public DefaultDictionary<TKey1, DefaultDictionary<TKey2, TValue>>.KeyCollection Keys
        {
            get
            {
                return dict.Keys;
            }
        }
    }
}
