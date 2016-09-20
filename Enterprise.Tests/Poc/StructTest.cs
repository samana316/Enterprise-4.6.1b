using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Enterprise.Core.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enterprise.Tests.Poc
{
    [TestClass]
    public sealed class StructTest
    {
        private const string CategoryPocStruct = "Poc.Struct";

        [TestMethod]
        [TestCategory(CategoryPocStruct)]
        public void StructEquality()
        {
            var first = new TestStruct<int, int>(1, 2);
            var second = new TestStruct<int, int>(1, 2);
            var third = new TestStruct<int, int>(2, 4);

            Assert.AreEqual(first, second);
            Assert.AreNotEqual(first, third);
            Assert.IsTrue(first.Equals(second));
            Assert.IsTrue(second.Equals((object)first));
            Assert.IsFalse(first.Equals(third));
            Assert.IsFalse(third.Equals((object)first));
        }

        private struct TestStruct<T1, T2> : IEquatable<TestStruct<T1, T2>>
        {
            public TestStruct(
                T1 value1, 
                T2 value2) 
                : this()
            {
                this.Value1 = value1;
                this.Value2 = value2;
            }

            public T1 Value1 { get; private set; }

            public T2 Value2 { get; private set; }

            public bool Equals(
                TestStruct<T1, T2> other)
            {
                return base.Equals(other);
            }
        }
    }
}
