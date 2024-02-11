using System;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Tests.Linq.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Enterprise.Core.Linq.AsyncEnumerable;

namespace Enterprise.Tests.Linq.SeekingOperations
{
    [TestClass]
    public class SeekingTest
    {
        private const string CategoryLinqFirstAsync = "Linq.FirstAsync";

        private const string CategoryLinqLastAsync = "Linq.LastAsync";

        private const string CategoryLinqSingleAsync = "Linq.SingleAsync";

        [TestMethod]
        [TestCategory(CategoryLinqFirstAsync)]
        [TestCategory(CategoryLinqLastAsync)]
        [TestCategory(CategoryLinqSingleAsync)]
        public async Task SingleElementNoPredicate()
        {
            var source = new RealAsyncEnumerable<int>(1);
            Assert.AreEqual(1, await source.FirstAsync());
            Assert.AreEqual(1, await source.FirstOrDefaultAsync());
            Assert.AreEqual(1, await source.SingleAsync());
            Assert.AreEqual(1, await source.SingleOrDefaultAsync());
            Assert.AreEqual(1, await source.LastAsync());
            Assert.AreEqual(1, await source.LastOrDefaultAsync());
        }

        [TestMethod]
        [TestCategory(CategoryLinqFirstAsync)]
        [TestCategory(CategoryLinqLastAsync)]
        [TestCategory(CategoryLinqSingleAsync)]
        public async Task SingleElementMatchPredicate()
        {
            var source = new RealAsyncEnumerable<int>(1);
            var predicate = new Func<int, bool>(x => x > 0);

            Assert.AreEqual(1, await source.FirstAsync(predicate));
            Assert.AreEqual(1, await source.FirstOrDefaultAsync(predicate));
            Assert.AreEqual(1, await source.SingleAsync(predicate));
            Assert.AreEqual(1, await source.SingleOrDefaultAsync(predicate));
            Assert.AreEqual(1, await source.LastAsync(predicate));
            Assert.AreEqual(1, await source.LastOrDefaultAsync(predicate));
        }

        [TestMethod]
        [TestCategory(CategoryLinqFirstAsync)]
        [TestCategory(CategoryLinqLastAsync)]
        [TestCategory(CategoryLinqSingleAsync)]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task SingleElementNotMatchPredicate()
        {
            var source = new RealAsyncEnumerable<int>(1);
            var predicate = new Func<int, bool>(x => x > 1);
            
            Assert.AreEqual(0, await source.FirstOrDefaultAsync(predicate));
            Assert.AreEqual(0, await source.SingleOrDefaultAsync(predicate));
            Assert.AreEqual(0, await source.LastOrDefaultAsync(predicate));

            await source.FirstAsync(predicate);
        }

        [TestMethod]
        [TestCategory(CategoryLinqFirstAsync)]
        [TestCategory(CategoryLinqLastAsync)]
        [TestCategory(CategoryLinqSingleAsync)]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task MultipleElementsNoPredicate()
        {
            var source = new RealAsyncEnumerable<int>(1, 2, 3);
            Assert.AreEqual(1, await source.FirstAsync());
            Assert.AreEqual(1, await source.FirstOrDefaultAsync());
            Assert.AreEqual(3, await source.LastAsync());
            Assert.AreEqual(3, await source.LastOrDefaultAsync());

            await source.SingleAsync();
        }

        [TestMethod]
        [TestCategory(CategoryLinqFirstAsync)]
        [TestCategory(CategoryLinqLastAsync)]
        [TestCategory(CategoryLinqSingleAsync)]
        public async Task MultipleElementsOneMatchPredicate()
        {
            var source = new RealAsyncEnumerable<int>(1, 2, 3);
            var predicate = new Func<int, bool>(x => x % 2 == 0);

            Assert.AreEqual(2, await source.FirstAsync(predicate));
            Assert.AreEqual(2, await source.FirstOrDefaultAsync(predicate));
            Assert.AreEqual(2, await source.SingleAsync(predicate));
            Assert.AreEqual(2, await source.SingleOrDefaultAsync(predicate));
            Assert.AreEqual(2, await source.LastAsync(predicate));
            Assert.AreEqual(2, await source.LastOrDefaultAsync(predicate));
        }

        [TestMethod]
        [TestCategory(CategoryLinqFirstAsync)]
        [TestCategory(CategoryLinqLastAsync)]
        [TestCategory(CategoryLinqSingleAsync)]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task MultipleElementsManyMatchPredicate()
        {
            var source = new RealAsyncEnumerable<int>(1, 2, 3, 4);
            var predicate = new Func<int, bool>(x => x > 1 && x < 4);

            Assert.AreEqual(2, await source.FirstAsync(predicate));
            Assert.AreEqual(2, await source.FirstOrDefaultAsync(predicate));
            Assert.AreEqual(3, await source.LastAsync(predicate));
            Assert.AreEqual(3, await source.LastOrDefaultAsync(predicate));

            await source.SingleOrDefaultAsync(predicate);
        }
    }
}
