using System;
using System.Collections.Generic;

namespace Engine.DataStructures
{
    /// <summary>
    /// Extensions that make certain lookup operations easier
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Tries to get a value from the dictionary.  If the dictionary
        /// returns a null value for the value, returns a default value
        /// </summary>
        public static TValue GetValueOrDefault<TKey, TValue>
            (this IDictionary<TKey, TValue> dictionary,
            TKey key, TValue defaultValue)
        {
            TValue value;
            return dictionary.TryGetValue(key, out value) ? value : defaultValue;
        }

        /// <summary>
        /// Tries to get a value from the dictionary.  If the dictionary
        /// returns a null value for the value, returns the value from
        /// a function which provides TValue objects
        /// </summary>
        public static TValue GetValueOrDefault<TKey, TValue>
                (this IDictionary<TKey, TValue> dictionary,
                 TKey key,
                 Func<TValue> defaultValueProvider)
        {
            TValue value;
            return dictionary.TryGetValue(key, out value) ? value
                 : defaultValueProvider();
        }

    }
}
