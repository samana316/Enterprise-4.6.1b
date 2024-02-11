using System;
using System.Linq;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Tests.Linq.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Enterprise.Core.Linq.AsyncEnumerable;

namespace Enterprise.Tests.Linq.SetOperations
{
    [TestClass]
    public sealed class UnionTest
    {
        private const string CategoryLinqUnion = "Linq.Union";

        [TestMethod]
        [TestCategory(CategoryLinqUnion)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateNullFirst()
        {
            IAsyncEnumerable<int> first = null;
            var query = first.Union(null);
        }

        [TestMethod]
        [TestCategory(CategoryLinqUnion)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateNullSecond()
        {
            var first = Empty<int>();
            var query = first.Union(null);
        }

        [TestMethod]
        [TestCategory(CategoryLinqUnion)]
        public async Task Simple()
        {
            var first = new RealAsyncEnumerable<string>("ABC");
            var second = new RealAsyncEnumerable<string>("ABC");

            var query = first.Union(second);
            Assert.IsTrue(await query.SequenceEqualAsync(first));
            Assert.IsTrue(await query.SequenceEqualAsync(second));
        }

        [TestMethod]
        [TestCategory(CategoryLinqUnion)]
        public async Task CaseInsensitiveOrdinalComparer()
        {
            var first = new RealAsyncEnumerable<string>("ABC");
            var second = new RealAsyncEnumerable<string>("abc");
            var comparer = StringComparer.OrdinalIgnoreCase;
            var query = first.Union(second, comparer);
            Assert.IsTrue(await query.SequenceEqualAsync(first, comparer));
            Assert.IsTrue(await query.SequenceEqualAsync(second, comparer));
        }

        [TestMethod]
        [TestCategory(CategoryLinqUnion)]
        public async Task EmptyFirstOnly()
        {
            var first = Empty<string>();
            var second = new RealAsyncEnumerable<string>("ABC");

            var query = first.Union(second);
            Assert.IsTrue(await query.SequenceEqualAsync(second));
        }

        [TestMethod]
        [TestCategory(CategoryLinqUnion)]
        public async Task EmptySecondOnly()
        {
            var first = new RealAsyncEnumerable<string>("ABC");
            var second = Empty<string>();

            var query = first.Union(second);
            Assert.IsTrue(await query.SequenceEqualAsync(first));
        }

        [TestMethod]
        [TestCategory(CategoryLinqUnion)]
        public async Task BothEmpty()
        {
            var first = Empty<string>();
            var second = Empty<string>();

            var query = first.Union(second);
            Assert.IsTrue(await query.SequenceEqualAsync(first));
            Assert.IsTrue(await query.SequenceEqualAsync(second));
        }

        [TestMethod]
        [TestCategory(CategoryLinqUnion)]
        [ExpectedException(typeof(NotImplementedException))]
        public async Task FirstSequenceIsntAccessedBeforeFirstUse()
        {
            var first = new ThrowAsyncEnumerable<int>();
            var second = new[] { 5 };

            var query = first.Union(second);

            using (var iterator = query.GetAsyncEnumerator())
            {
                await iterator.MoveNextAsync();
            }
        }

        [TestMethod]
        [TestCategory(CategoryLinqUnion)]
        [ExpectedException(typeof(NotImplementedException))]
        public async Task SecondSequenceIsntAccessedBeforeFirstUse()
        {
            var first = new RealAsyncEnumerable<int>(5);
            var second = new ThrowAsyncEnumerable<int>();

            var query = first.Union(second);

            using (var iterator = query.GetAsyncEnumerator())
            {
                Assert.IsTrue(await iterator.MoveNextAsync());
                Assert.AreEqual(5, iterator.Current);

                await iterator.MoveNextAsync();
            }
        }
    }
}
