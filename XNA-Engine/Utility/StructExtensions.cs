using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Utility
{
    /// <summary>
    /// Extensions for common operations like full-array copying
    /// </summary>
    public static class StructExtensions
    {
        /// <summary>
        /// Returns a copy of the entirety of the source array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T[] Copy<T>(this T[] source) where T : struct
        {
            T[] t = new T[source.Length];
            Array.Copy(source, t, source.Length);
            return t;
        }
    }
}
