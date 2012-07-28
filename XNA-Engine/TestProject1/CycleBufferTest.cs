using Engine.DataStructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Engine.Input;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for CycleBufferTest and is intended
    ///to contain all CycleBufferTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CycleBufferTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for CycleBuffer`2 Constructor
        ///</summary>
        public void CycleBufferConstructorTestHelper<TKey, TValue>()
            where TKey : struct
        {
            TKey keyCurrent = new TKey(); // TODO: Initialize to an appropriate value
            TKey keyPrevious = new TKey(); // TODO: Initialize to an appropriate value
            var target = new CycleBuffer<TKey, List<TValue>, TValue>(keyCurrent, keyPrevious);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        [TestMethod()]
        public void CycleBufferConstructorTest()
        {
            FrameState current = FrameState.Current;
            FrameState previous = FrameState.Previous;
            var input = new CycleBuffer<FrameState, List<Keys>, Keys>(current, previous);
            input[current].Add(Keys.A);
            input.Add(previous, Keys.B);

            Assert.AreEqual<Keys>(Keys.B, input[previous][0]);
            Assert.AreEqual<Keys>(Keys.A, input[current][0]);

            input.Cycle();

            Assert.AreEqual<Keys>(Keys.A, input[previous][0]);
            Assert.IsTrue(input[current].Count == 0);
        }
    }
}
