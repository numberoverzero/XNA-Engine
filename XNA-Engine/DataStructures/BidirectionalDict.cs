using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

using Engine.Utility;

namespace Engine.DataStructures
{
    /// <summary>
    /// Provides strict 1:1 mapping between two types.
    /// Not thread safe by any stretch of the imagination
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public class BidirectionalDict<T1, T2>
    {
        ConcurrentDictionary<T1, T2> t1_t2;
        ConcurrentDictionary<T2, T1> t2_t1;

        /// <summary>
        /// Empty Bidirectional Dictionary
        /// </summary>
        public BidirectionalDict()
        {
            t1_t2 = new ConcurrentDictionary<T1, T2>();
            t2_t1 = new ConcurrentDictionary<T2, T1>();
        }

        /// <summary>
        /// Construct the equivalent bi-directional dictionary from a single-direction dictionary
        /// </summary>
        /// <param name="dictionary"></param>
        public BidirectionalDict(IDictionary<T1, T2> dictionary)
        {
            t1_t2 = new ConcurrentDictionary<T1, T2>(dictionary);
            t2_t1 = new ConcurrentDictionary<T2, T1>();
            foreach (T1 t1 in dictionary.Keys)
                t2_t1[dictionary[t1]] = t1;
        }

        /// <summary>
        /// Construct the equivalent bi-directional dictionary from a T2 -> T1 dictionary
        /// </summary>
        /// <param name="dictionary"></param>
        public BidirectionalDict(IDictionary<T2, T1> dictionary)
        {
            t2_t1 = new ConcurrentDictionary<T2, T1>(dictionary);
            t1_t2 = new ConcurrentDictionary<T1, T2>();
            foreach (T2 t2 in dictionary.Keys)
                t1_t2[dictionary[t2]] = t2;
        }

        /// <summary>
        /// Gets or sets the values associated with the specified key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T1 this[T2 key]
        {
            get
            {
                return t2_t1[key];
            }
            set
            {
                t2_t1[key] = value;
                t1_t2[value] = key;
            }
        }

        /// <summary>
        /// Gets or sets the values associated with the specified key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T2 this[T1 key]
        {
            get
            {
                return t1_t2[key];
            }
            set
            {
                t1_t2[key] = value;
                t2_t1[value] = key;
            }
        }

        /// <summary>
        /// Add the specified pairing to the dictionary
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(T1 key, T2 value)
        {
            this[key] = value;
        }
        /// <summary>
        /// Add the specified pairing to the dictionary
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(T2 key, T1 value)
        {
            Add(value, key);
        }

        /// <summary>
        /// Remove the item and its corresponding value from the dictionary
        /// </summary>
        /// <param name="k"></param>
        public void Remove(T1 k)
        {
            Remove(k, this[k]);
        }
        /// <summary>
        /// Remove the item and its corresponding value from the dictionary
        /// </summary>
        /// <param name="k"></param>
        public void Remove(T2 k)
        {
            Remove(this[k], k);
        }

        private void Remove(T1 key, T2 value)
        {
            if (t1_t2.ContainsKey(key) || t2_t1.ContainsKey(value))
            {
                t1_t2.Remove(key);
                t2_t1.Remove(value);
            }
        }

        /// <summary>
        /// True if there is a pairing with the given value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool HasItem(T1 key)
        {
            return t1_t2.ContainsKey(key);
        }
        /// <summary>
        /// True if there is a pairing with the given value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool HasItem(T2 key)
        {
            return t2_t1.ContainsKey(key);
        }

        /// <summary>
        /// Returns the values of the first type specification in the table
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T1> GetValuesType1()
        {
            return t1_t2.Keys;
        }

        /// <summary>
        /// Returns the values of the second type specification in the table
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T2> GetValuesType2()
        {
            return t2_t1.Keys;
        }

        
    }
}
