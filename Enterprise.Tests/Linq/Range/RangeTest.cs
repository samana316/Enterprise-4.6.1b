using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enterprise.Tests.Linq.Range
{
    [TestClass]
    public class RangeTest
    {
        private const int DefaultTimeout = 1000;
        private const string CategoryLinqRange = "Linq.Range";

        [TestMethod]
        [TestCategory(CategoryLinqRange)]
        public async Task SimpleValidRange()
        {
            var source = AsyncEnumerable.Range(1, 3);
            Assert.IsTrue(await source.SequenceEqualAsync(new[] { 1, 2, 3 }));
        }

        [TestMethod]
        [TestCategory(CategoryLinqRange)]
        public async Task AllowNegativeStart()
        {
            var source = AsyncEnumerable.Range(-1, 3);
            Assert.IsTrue(await source.SequenceEqualAsync(new[] { -1, 0, 1 }));
        }

        [TestMethod]
        [TestCategory(CategoryLinqRange)]
        public async Task ZeroCountShouldBeEmpty()
        {
            var source = AsyncEnumerable.Range(int.MinValue, 0);
            Assert.IsTrue(await source.SequenceEqualAsync(new int[0]));
        }

        [TestMethod]
        [TestCategory(CategoryLinqRange)]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CountCantBeNegative()
        {
            var source = AsyncEnumerable.Range(int.MinValue, -1);
        }

        [TestMethod]
        [TestCategory(CategoryLinqRange)]
        public async Task CanIncludeMaxValue()
        {
            var source = AsyncEnumerable.Range(int.MaxValue, 1);
            Assert.IsTrue(await source.SequenceEqualAsync(new[] { int.MaxValue }));
        }

        [TestMethod]
        [TestCategory(CategoryLinqRange)]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CantExceedMaxValue()
        {
            var source = AsyncEnumerable.Range(int.MaxValue, 2);
        }

        [TestMethod]
        [TestCategory(CategoryLinqRange)]
        [Timeout(DefaultTimeout)]
        public void SmallStartWithLargeCount()
        {
            var count = 100000;
            var source = AsyncEnumerable.Range(1, count);
            var list = new List<int>(count);
            list.AddRange(source);
            Assert.AreEqual(count, list.Count);

            var expected = Enumerable.Range(1, count).ToList();
            CollectionAssert.AreEquivalent(expected, list);
        }
    }
}
