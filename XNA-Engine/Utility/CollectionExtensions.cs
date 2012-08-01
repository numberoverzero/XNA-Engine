using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Utility
{
    /// <summary>
    /// Provides iteration over the values of an enumeration
    /// </summary>
    public static class EnumUtil
    {
        /// <summary>
        /// An iterator over the values of an enumeration
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }

    /// <summary>
    /// Extensions for commonly used collection patterns
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Takes an enumerable and returns the string such that each element is printed,
        /// separated by the given separator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string PrettyPrint<T>(this IEnumerable<T> enumerable, string separator)
        {
            var sb = new StringBuilder();
            foreach (var t in enumerable) { sb.Append(t); sb.Append(separator); }
            return sb.ToString();
        }

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
