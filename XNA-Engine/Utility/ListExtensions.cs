using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Utility
{
    public static class ListExtensions
    {
        /// <summary>
        /// Extension because I hate writing new List&lt;T&gt;(list)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<T> Copy<T>(this List<T> list)
        {
            return new List<T>(list);
        }

        /// <summary>
        /// Return a copy of the list, sorted using the default comparer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<T> Sorted<T>(this List<T> list)
        {
            var copy = list.Copy();
            copy.Sort();
            return copy;
        }

        /// <summary>
        /// Return a copy of the list, sorted using the given comparer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<T> Sorted<T>(this List<T> list, IComparer<T> comparer )
        {
            var copy = list.Copy();
            copy.Sort(comparer);
            return copy;
        }

        
    }
}
