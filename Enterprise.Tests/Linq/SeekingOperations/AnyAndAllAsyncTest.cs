using System;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Tests.Linq.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Enterprise.Core.Linq.AsyncEnumerable;

namespace Enterprise.Tests.Linq.AnyAllAsync
{
    [TestClass]
    public sealed class AnyAndAllAsyncTest
    {
        private const string CategoryLinqAnyAsync = "Linq.AnyAsync";

        private const string CategoryLinqAllAsync = "Linq.AllAsync";

        [TestMethod]
        [TestCategory(CategoryLinqAnyAsync)]
        [TestCategory(CategoryLinqAllAsync)]
        public async Task EmptySequence()
        {
            var source = Empty<int>();
            var predicate = new Func<int, bool>(x => x > 0);

            Assert.IsFalse(await source.AnyAsync());
            Assert.IsFalse(await source.AnyAsync(predicate));
            Assert.IsTrue(await source.AllAsync(predicate));
        }

        [TestMethod]
        [TestCategory(CategoryLinqAnyAsync)]
        public async Task NonEmptyWithoutPredicate()
        {
            var source = new RealAsyncEnumerable<int>(1);

            Assert.IsTrue(await source.AnyAsync());
        }

        [TestMethod]
        [TestCategory(CategoryLinqAnyAsync)]
        [TestCategory(CategoryLinqAllAsync)]
        public async Task AllElementsNotMatchPredicate()
        {
            var source = new RealAsyncEnumerable<int>(1, 2, 3);
            var predicate = new Func<int, bool>(x => x < 0);
            
            Assert.IsFalse(await source.AnyAsync(predicate));
            Assert.IsFalse(await source.AllAsync(predicate));
        }

        [TestMethod]
        [TestCategory(CategoryLinqAnyAsync)]
        [TestCategory(CategoryLinqAllAsync)]
        public async Task SomeElementsMatchPredicate()
        {
            var source = new RealAsyncEnumerable<int>(1, 2, 3);
            var predicate = new Func<int, bool>(x => x > 1);

            Assert.IsTrue(await source.AnyAsync(predicate));
            Assert.IsFalse(await source.AllAsync(predicate));
        }

        [TestMethod]
        [TestCategory(CategoryLinqAnyAsync)]
        [TestCategory(CategoryLinqAllAsync)]
        public async Task AllElementsMatchPredicate()
        {
            var source = new RealAsyncEnumerable<int>(1, 2, 3);
            var predicate = new Func<int, bool>(x => x > 0);

            Assert.IsTrue(await source.AnyAsync(predicate));
            Assert.IsTrue(await source.AllAsync(predicate));
        }
    }
}
