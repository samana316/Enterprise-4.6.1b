using System;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Tests.Linq.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Enterprise.Core.Linq.AsyncEnumerable;

namespace Enterprise.Tests.Linq.ElementAt
{
    [TestClass]
    public sealed class ElementAtAsyncTest
    {
        private const string CategoryLinqElementAt = "Linq.ElementAtAsync";

        [TestMethod]
        [TestCategory(CategoryLinqElementAt)]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task NullSource()
        {
            var source = default(IAsyncEnumerable<int>);

            var task1 = source.ElementAtAsync(0);
            var task2 = source.ElementAtOrDefaultAsync(0);

            await Task.WhenAll(task1, task2);
        }

        [TestMethod]
        [TestCategory(CategoryLinqElementAt)]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public async Task NegativeIndex()
        {
            var source = new ThrowAsyncEnumerable<int>();
            var result1 = await source.ElementAtOrDefaultAsync(-1);
            Assert.AreEqual(default(int), result1);

            var result2 = await source.ElementAtAsync(-1);
        }

        [TestMethod]
        [TestCategory(CategoryLinqElementAt)]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public async Task IndexTooBigEager()
        {
            var source = new[] { 1, 2, 3 }.AsAsyncEnumerable();
            var result1 = await source.ElementAtOrDefaultAsync(4);
            Assert.AreEqual(2, result1);

            var result2 = await source.ElementAtAsync(4);
            Assert.AreEqual(2, result2);
        }

        [TestMethod]
        [TestCategory(CategoryLinqElementAt)]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public async Task IndexTooBigLazy()
        {
            var source = new RealAsyncEnumerable<int>(1, 2, 3);
            var result1 = await source.ElementAtOrDefaultAsync(4);
            Assert.AreEqual(default(int), result1);

            var result2 = await source.ElementAtAsync(4);
        }

        [TestMethod]
        [TestCategory(CategoryLinqElementAt)]
        public async Task ValidIndexEager()
        {
            var index = 1;

            var source = new[] { 1, 2, 3 };
            var query = source.AsAsyncEnumerable();
            var result1 = await query.ElementAtOrDefaultAsync(index);
            Assert.AreEqual(source[index], result1);

            var result2 = await query.ElementAtAsync(index);
            Assert.AreEqual(source[index], result2);
        }

        [TestMethod]
        [TestCategory(CategoryLinqElementAt)]
        public async Task ValidIndexLazy()
        {
            var source = new RealAsyncEnumerable<int>(1, 2, 3);
            var result1 = await source.ElementAtOrDefaultAsync(1);
            Assert.AreEqual(2, result1);

            var result2 = await source.ElementAtAsync(1);
            Assert.AreEqual(2, result2);
        }
    }
}
