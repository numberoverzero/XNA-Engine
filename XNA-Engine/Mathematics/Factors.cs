using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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


        public static long LargestProduct(List<long> numbers, long upperLimit)
        {
            var sortedNumbers = numbers.Sorted();
            throw new NotImplementedException();
            return 0;
        }
    }
}
