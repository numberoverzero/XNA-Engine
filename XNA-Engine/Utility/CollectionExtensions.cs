using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Utility
{
    /// <summary>
    /// Extensions for commonly used collection patterns
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Selects a random element from the collection, removes and returns it
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static T PopRandomElement<T>(this ICollection<T> collection)
        {
            if (collection.Count == 0) return default(T);
            int index = Guid.NewGuid().GetHashCode() % collection.Count;
            var iter = collection.GetEnumerator();
            int i = 0; while (i <= index) { iter.MoveNext(); i++; }
            var t = iter.Current;
            collection.Remove(t);
            return t;
        }
    }
}
