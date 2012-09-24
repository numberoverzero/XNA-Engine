using System.Collections.Generic;

namespace Engine.DataStructures
{
    public class Dictionary<TKey1, TKey2, TValue>
    {
        private readonly Dictionary<TKey1, Dictionary<TKey2, TValue>> _dict;

        public Dictionary()
        {
            _dict = new Dictionary<TKey1, Dictionary<TKey2, TValue>>();
        }

        public Dictionary<TKey2, TValue> this[TKey1 key1]
        {
            get
            {
                if (!_dict.ContainsKey(key1))
                    _dict[key1] = new Dictionary<TKey2, TValue>();
                return _dict[key1];
            }
            set { _dict[key1] = value; }
        }

        public TValue this[TKey1 key1, TKey2 key2]
        {
            get { return this[key1][key2]; }
            set { this[key1][key2] = value; }
        }

        public bool ContainsKey(TKey1 key1, TKey2 key2)
        {
            return this[key1].ContainsKey(key2);
        }
    }
}