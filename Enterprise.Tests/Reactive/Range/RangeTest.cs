using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Reactive.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enterprise.Tests.Reactive.Range
{
    [TestClass]
    public class RangeTest
    {
        private const int DefaultTimeout = 1000;
        private const string CategoryReactiveRange = "Reactive.Range";

        [TestMethod]
        [TestCategory(CategoryReactiveRange)]
        public async Task SimpleValidRange()
        {
            var source = AsyncObservable.Range(1, 3);
            Assert.IsTrue(await source.SequenceEqual(new[] { 1, 2, 3 }));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveRange)]
        public async Task AllowNegativeStart()
        {
            var source = AsyncObservable.Range(-1, 3);
            Assert.IsTrue(await source.SequenceEqual(new[] { -1, 0, 1 }));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveRange)]
        public async Task ZeroCountShouldBeEmpty()
        {
            var source = AsyncObservable.Range(int.MinValue, 0);
            Assert.IsTrue(await source.SequenceEqual(new int[0]));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveRange)]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CountCantBeNegative()
        {
            var source = AsyncObservable.Range(int.MinValue, -1);
        }

        [TestMethod]
        [TestCategory(CategoryReactiveRange)]
        public async Task CanIncludeMaxValue()
        {
            var source = AsyncObservable.Range(int.MaxValue, 1);
            Assert.IsTrue(await source.SequenceEqual(new[] { int.MaxValue }));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveRange)]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CantExceedMaxValue()
        {
            var source = AsyncObservable.Range(int.MaxValue, 2);
        }

        [TestMethod]
        [TestCategory(CategoryReactiveRange)]
        [Timeout(DefaultTimeout)]
        public void SmallStartWithLargeCount()
        {
            var count = 100000;
            var source = AsyncObservable.Range(1, count);
            var list = new List<int>(count);
            list.AddRange(source.ToEnumerable());
            Assert.AreEqual(count, list.Count);

            var expected = Enumerable.Range(1, count).ToList();
            CollectionAssert.AreEquivalent(expected, list);
        }
    }
}
