using System.Collections.Generic;
using System.Linq;

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
    }
}