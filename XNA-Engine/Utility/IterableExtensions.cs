using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Utility
{
    /// <summary>
    ///   Extensions for generic Iterables
    /// </summary>
    public static class IterableExtensions
    {
        /// <summary>
        ///   Return r-length subsequences of elements from the input iterable.
        ///   Combinations are emitted in lexicographic sort order.
        ///   Elements are treated as unique based on their position, not on their value.
        /// </summary>
        /// <example>
        ///   'ABCD'.Combinations(2) --> AB AC AD BC BD CD
        /// </example>
        public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> elements, int k)
        {
            if (elements == null) return null;
            var enumerable = elements as List<T> ?? elements.ToList();
            return k == 0
                       ? new[] {new T[0]}
                       : enumerable.SelectMany((e, i) =>
                                               enumerable.Skip(i + 1).Combinations(k - 1).Select(
                                                   c => (new[] {e}).Concat(c)));
        }

        /// <summary>
        ///   Return r-length subsequences (as lists) of elements from the input iterable.
        ///   Combinations are emitted in lexicographic sort order.
        ///   Elements are treated as unique based on their position, not on their value.
        /// </summary>
        /// <example>
        ///   'ABCD'.Combinations(2) --> AB AC AD BC BD CD
        /// </example>
        public static IEnumerable<List<T>> ListCombinations<T>(this IEnumerable<T> elements, int k)
        {
            var combinations = elements.Combinations(k);
            return combinations.Select(result => result.ToList());
        }

        /// <summary>
        /// Compact foreach extension
        /// </summary>
        public static void Each<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (T t in enumerable)
                action(t);
        }

        /// <summary>
        /// Compact foreach extension
        /// </summary>
        public static void Each<T>(this T[] enumerable, Action<T> action)
        {
            foreach (T t in enumerable)
                action(t);
        }

        /// <summary>
        /// Returns the last element of a list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T Last<T>(this List<T> list )
        {
            return list[list.Count-1];
        }

        /// <summary>
        /// Does an action n times
        /// </summary>
        public static void TimesDo(this int repeat, Action action)
        {
            for (var i = 0; i < repeat; i++) action();
        }

        public static IEnumerable<T> Mutate<T>(this IEnumerable<T> enumerable, Func<T, T> mutator)
        {
            return enumerable.Select(mutator);
        }

        public static string PrettyPrint<T>(this IEnumerable<T> enumerable, char delim = ',')
        {
            var enumCopy = enumerable.ToList();
            var n = enumCopy.Count();
            var sb = new StringBuilder();
            sb.Append('[');
            foreach (var t in enumCopy.Take(n - 1))
            {
                sb.Append(t.ToString());
                sb.Append(delim);
            }
            sb.Append(enumCopy.Last());
            sb.Append(']');
            return sb.ToString();
        }
    }
}