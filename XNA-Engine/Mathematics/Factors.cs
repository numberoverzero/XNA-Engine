using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Mathematics
{
    public static class Factorization
    {
        /// <summary>
        ///     <para>Returns a stream of factors of a number in increasing magnitude,</para>
        ///     <para>such that Factors[i] &lt;= Factors[i+1]</para>
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static IEnumerable<long> Factors(long n)
        {
            if (n < 0) n *= -1;
            long f = 1;
            while (n > 1)
            {
                f++;
                if (n % f != 0) continue;
                yield return f;
                n /= f;
                f = 1;
            }
        }

        /// <summary>
        /// Returns the largest product of numbers that is less than or equal to the upper limit.
        /// The current method is most likely impressively suboptimal.
        /// </summary>
        /// <param name="numbers">List of numbers to create a product from</param>
        /// <param name="upperLimit">A number which the product must be smaller than.</param>
        /// <returns>The largest value (less than or equal to upperLimit) which can be formed as a product of values from the given list of numbers</returns>
        public static long LargestProduct(List<long> numbers, long upperLimit)
        {
            var maxProduct = numbers.Aggregate((product, factor) => product * factor);
            while (upperLimit > 1)
            {
                if (Basics.GCD(upperLimit, maxProduct) == upperLimit)
                    return upperLimit;
                upperLimit--;
            }
            return 1;
        }
    }
}
