using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Utility
{
    public class DefaultDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
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
    public class CountedSet<T> : IList<T>
    {
        private List<T> set;
        private DefaultDictionary<T, int> count;

        public CountedSet()
        {
            set = new List<T>();
            count = new DefaultDictionary<T, int>();
        }

        public CountedSet(IList<T> enumerable) : this()
        {
            foreach (T item in enumerable)
                Add(item);
        }

        public CountedSet(CountedSet<T> countedSet)
        {
            set = new List<T>(countedSet.set);
            count = new DefaultDictionary<T, int>(countedSet.count);
        }

        #region IList NotImplementedException

        public int IndexOf(T item)
        {
            throw new NotImplementedException("Sets do not support ordering.");
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException("Sets do not support ordering.");
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException("Sets do not support ordering.");
        }

        public T this[int index]
        {
            get
            {
                throw new NotImplementedException("Sets do not support ordering.");
            }
            set
            {
                throw new NotImplementedException("Sets do not support ordering.");
            }
        }

        #endregion

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

        public void CopyTo(T[] array, int arrayIndex)
        {
            set.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return set.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
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
}
