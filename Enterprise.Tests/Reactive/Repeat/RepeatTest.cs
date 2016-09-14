using System;
using System.Linq;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Core.Reactive.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enterprise.Tests.Reactive.Repeat
{
    [TestClass]
    public class RepeatTest
    {
        private const string CategoryReactiveRepeat = "Reactive.Repeat";

        [TestMethod]
        [TestCategory(CategoryReactiveRepeat)]
        public async Task Simple()
        {
            var source = AsyncObservable.Repeat(1, 3);
            Assert.IsTrue(await source.SequenceEqual(new[] { 1, 1, 1 }));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveRepeat)]
        public async Task Empty()
        {
            var source = AsyncObservable.Repeat(1, 0);
            Assert.IsTrue(await source.SequenceEqual(new int[0]));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveRepeat)]
        public async Task Null()
        {
            var source = AsyncObservable.Repeat<object>(null, 3);
            Assert.IsTrue(await source.SequenceEqual(new object[] { null, null, null }));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveRepeat)]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CountCantBeNegative()
        {
            var source = AsyncObservable.Repeat(int.MinValue, -1);
        }

        [TestMethod]
        [TestCategory(CategoryReactiveRepeat)]
        public async Task Infinite()
        {
            var source = AsyncObservable.Repeat(1);
            var query = source.ToAsyncEnumerable().Take(3);

            Assert.IsTrue(await query.SequenceEqualAsync(new[] { 1, 1, 1 }));
        }
    }
}
