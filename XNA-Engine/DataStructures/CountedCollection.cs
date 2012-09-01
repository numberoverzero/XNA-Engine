using System.Collections;
using System.Collections.Generic;

namespace Engine.DataStructures
{
    /// <summary>
    ///   Provides an iterable set of unique entries.
    ///   Adding an item increases the count of that item by 1.
    ///   Removing an item decreases the count by 1.
    ///   When the count is zero, the value is removed from the set.
    /// 
    ///   The iterator is over unique values
    /// </summary>
    /// <typeparam name="T"> </typeparam>
    public class CountedCollection<T> : ICollection<T>
    {
        private readonly DefaultDict<T, int> count;
        private readonly HashSet<T> set;

        /// <summary>
        ///   Construct an empty CountedSet
        /// </summary>
        public CountedCollection()
        {
            set = new HashSet<T>();
            count = new DefaultDict<T, int>();
        }

        /// <summary>
        ///   Copy the values of an enumerable into a CountedSet.
        ///   Note that duplicates will increment the count, and are not included in the iterable set twice.
        /// </summary>
        /// <param name="enumerable"> </param>
        public CountedCollection(IEnumerable<T> enumerable)
            : this()
        {
            foreach (T item in enumerable)
                ((ICollection<T>) this).Add(item);
        }

        /// <summary>
        ///   Copy the values of another counted set, as well as the count for each item.
        /// </summary>
        /// <param name="countedSet"> </param>
        public CountedCollection(CountedCollection<T> countedSet)
        {
            set = new HashSet<T>(countedSet.set);
            count = new DefaultDict<T, int>(countedSet.count);
        }

        /// <summary>
        /// Merges the values of other into this collection, including their counts
        /// </summary>
        /// <param name="other"></param>
        public void Merge(CountedCollection<T> other )
        {
            foreach(T t in other.set)
                Add(t);
        }

        #region ICollection<T> Members

        /// <summary>
        ///   See ICollection.Add
        /// </summary>
        public void Add(T item)
        {
            if (count[item] <= 0)
                set.Add(item);
            count[item]++;
        }

        /// <summary>
        ///   See ICollection.Clear
        /// </summary>
        public void Clear()
        {
            set.Clear();
            count.Clear();
        }

        /// <summary>
        ///   See ICollection.Contains
        /// </summary>
        public bool Contains(T item)
        {
            return set.Contains(item);
        }

        /// <summary>
        ///   See ICollection.CopyTo
        /// </summary>
        public void CopyTo(T[] array, int arrayIndex)
        {
            set.CopyTo(array, arrayIndex);
        }

        /// <summary>
        ///   See ICollection.Count
        /// </summary>
        public int Count
        {
            get { return set.Count; }
        }

        /// <summary>
        ///   See ICollection.IsReadOnly
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        ///   See ICollection.Remove
        /// </summary>
        public bool Remove(T item)
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
            return ((IEnumerable<T>) this).GetEnumerator();
        }

        #endregion
    }
}