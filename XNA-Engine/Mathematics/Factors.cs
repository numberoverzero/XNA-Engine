using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Utility;

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
            yield return 1; 
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
        /// Returns the product of each set of the power set of a list of numbers.
        /// 
        /// Example:
        /// input [2,3,5]
        /// powerset is:
        /// [
        /// 	[2],
        /// 	[3],
        /// 	[2,3],
        /// 	[5],
        /// 	[2,5],
        /// 	[3,5],
        /// 	[2,3,5]
        /// ]
        /// productset is product of each list, or:
        /// [
        ///     2,
        ///     3,
        ///     6,
        ///     5,
        ///     10,
        ///     15,
        ///     30
        /// ]
        /// </summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
        public static IEnumerable<long> ProductSet(List<long> numbers)
        {
            return from powerSet in numbers.GetPowerSet() select powerSet.ToList() into factors where factors.Count != 0 select product(factors);
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
            var maxProduct = product(numbers);
            upperLimit = Math.Min(maxProduct, upperLimit);
            while (upperLimit > 1)
            {
                if (Basics.GCD(upperLimit, maxProduct) == upperLimit)
                    return upperLimit;
                upperLimit--;
            }
            return 1;
        }

        private static long product(IEnumerable<long> numbers)
        {
            return numbers.Aggregate((p, f) => p*f);
        }
    }
}
