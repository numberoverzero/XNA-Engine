using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Utility
{
    /// <summary>
    /// Extensions for manipulating dictionaries, such as merging
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Merges any number of dictionaries into a new one.
        /// Values in the new dictionary are overwritten left to right,
        /// such that order matters. (The dictionary this is performed on is the base,
        /// so any key that it shares with another dictionary will be overwritten)
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="others"></param>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> Merge<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary, 
            params IDictionary<TKey, TValue>[] others)
        {
            var result = new Dictionary<TKey, TValue>();
            foreach (var dict in (new List<IDictionary<TKey, TValue>> {dictionary}).Concat(others))
                foreach (var x in dict)
                    result[x.Key] = x.Value;
            return result;
        }

        /// <summary>
        /// When you're removing from a concurrent dictionary and you don't care if the value is successfully removed
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        public static void Remove<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue v;
            dictionary.TryRemove(key, out v);
        }
    }
}
