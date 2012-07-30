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
    public class CountedSet<T> : IEnumerable<T>
    {
        private HashSet<T> set;
        private DefaultDict<T, int> count;

        /// <summary>
        /// Construct an empty CountedSet
        /// </summary>
        public CountedSet()
        {
            set = new HashSet<T>();
            count = new DefaultDict<T, int>();
        }

        /// <summary>
        /// Copy the values of an enumerable into a CountedSet.
        /// Note that duplicates will increment the count, and are not included in the iterable set twice.
        /// </summary>
        /// <param name="enumerable"></param>
        public CountedSet(IEnumerable<T> enumerable)
            : this()
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
            count = new DefaultDict<T, int>(countedSet.count);
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
                {
                    count[item] = 0;
                    return set.Remove(item);
                }
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
}
