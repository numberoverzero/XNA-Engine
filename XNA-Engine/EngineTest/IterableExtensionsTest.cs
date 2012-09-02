using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EngineTest
{
    [TestClass]
    public class IterableExtensionsTest
    {
        [TestMethod]
        public void TestCombinationInt()
        {
            var input = NewList(0, 1, 2, 3);
            var expected = new List<List<int>> {NewList(0, 1, 2), NewList(0, 1, 3), NewList(0, 2, 3), NewList(1, 2, 3)};
            var actual = input.Combinations(3);
            foreach (var result in actual.Select(a => a.ToList()))
            {
                WriteList(result);
            }
        }

        [TestMethod]
        public void TestListCombinationInt()
        {
            var input = NewList(0, 1, 2, 3);
            var expected = new List<List<int>> { NewList(0, 1, 2), NewList(0, 1, 3), NewList(0, 2, 3), NewList(1, 2, 3) };
            var actual = input.ListCombinations(3);
            foreach (var result in actual)
            {
                WriteList(result);
            }
        }

        private List<T> NewList<T>(params T[] ts)
        {
            return new List<T>(ts);
        }

        private void WriteList<T> (IEnumerable<T> list)
        {
            string s = "[";
            foreach (var t in list)
                s += " {0},".format(t);
            s = s.Substring(0, s.Length - 1);
            s += "]";
            Console.WriteLine(s);
        }
    }
}