using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.DataStructures
{
    /// <summary>
    /// Provides an iterable set of unique entries.
    /// Adding an item increases the count of that item by 1.
    /// Removing an item decreases the count by 1.
    /// When the count is zero, the value is removed from the set.
    /// 
    /// The iterator is over unique values
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CountedCollection<T> : ICollection<T>
    {
        private HashSet<T> set;
        private DefaultDict<T, int> count;

        /// <summary>
        /// Construct an empty CountedSet
        /// </summary>
        public CountedCollection()
        {
            set = new HashSet<T>();
            count = new DefaultDict<T, int>();
        }

        /// <summary>
        /// Copy the values of an enumerable into a CountedSet.
        /// Note that duplicates will increment the count, and are not included in the iterable set twice.
        /// </summary>
        /// <param name="enumerable"></param>
        public CountedCollection(IEnumerable<T> enumerable)
            : this()
        {
            foreach (T item in enumerable)
                ((ICollection<T>)this).Add(item);
        }

        /// <summary>
        /// Copy the values of another counted set, as well as the count for each item.
        /// </summary>
        /// <param name="countedSet"></param>
        public CountedCollection(CountedCollection<T> countedSet)
        {
            set = new HashSet<T>(countedSet.set);
            count = new DefaultDict<T, int>(countedSet.count);
        }

        void ICollection<T>.Add(T item)
        {
            if (count[item] <= 0)
                set.Add(item);
            count[item]++;
        }

        void ICollection<T>.Clear()
        {
            set.Clear();
            count.Clear();
        }

        bool ICollection<T>.Contains(T item)
        {
            return set.Contains(item);
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            set.CopyTo(array, arrayIndex);
        }

        int ICollection<T>.Count
        {
            get { return set.Count; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<T>.Remove(T item)
        {
            if (set.Contains(item))
            {
                count[item]--;
                if (count[item] < 1)
                {
                    count[item] = 0;
                    return set.Remove(item);
                }
            }
            // If the set doesn't contain the item or there was more than one reference to the item, we didn't remove it.
            return false;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return set.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)this).GetEnumerator();
        }
    }
}
