using System;
using System.Collections.Generic;

namespace Engine.DataStructures
{
    /// <summary>
    ///   A DefaultDict that supports two keys.
    /// </summary>
    /// <typeparam name="TKey1"> The first key type </typeparam>
    /// <typeparam name="TKey2"> The second key type </typeparam>
    /// <typeparam name="TValue"> The value type stored in the dictionary </typeparam>
    public class MultiKeyObjDict<TKey1, TKey2, TValue> where TValue : new()
    {
        private readonly DefaultDict<TKey1, DefaultDict<TKey2, TValue>> _dict;

        /// <summary>
        ///   Construct an empty Double-keyed dictionary
        /// </summary>
        public MultiKeyObjDict()
        {
            Func<TValue> defaultInnerDictFunc = () => new TValue();
            Func<DefaultDict<TKey2, TValue>> defaultDictFunc =
                () => new DefaultDict<TKey2, TValue>(defaultInnerDictFunc);
            _dict = new DefaultDict<TKey1, DefaultDict<TKey2, TValue>>(defaultDictFunc);
        }

        /// <summary>
        ///   Copy Constructor
        /// </summary>
        /// <param name="defaultMultiKeyDict"> </param>
        public MultiKeyObjDict(MultiKeyObjDict<TKey1, TKey2, TValue> defaultMultiKeyDict)
        {
            _dict = new DefaultDict<TKey1, DefaultDict<TKey2, TValue>>(defaultMultiKeyDict._dict);
        }

        /// <summary>
        ///   Gets or sets the value associated with the specified keys.
        /// </summary>
        /// <param name="key1"> The first key of the value to get or set. </param>
        /// <param name="key2"> The second key of the value to get or set. </param>
        /// <returns> </returns>
        public TValue this[TKey1 key1, TKey2 key2]
        {
            get { return _dict[key1][key2]; }
            set { _dict[key1][key2] = value; }
        }

        /// <summary>
        ///   Gets a collection containing the keys in the Engine.Utility.DefaultMultiKeyDict&lt;TKey1, TKey2, TValue&gt;.
        /// </summary>
        public IEnumerable<TKey1> Keys
        {
            get { return _dict.Keys; }
        }

        /// <summary>
        ///   Clear all values from the DefaultMultiKeyDict
        /// </summary>
        public void Clear()
        {
            _dict.Clear();
        }
    }
}