using System;
using System.Linq;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Enterprise.Core.Linq.AsyncEnumerable;

namespace Enterprise.Tests.Linq.Reverse
{
    [TestClass]
    public sealed class ReverseTest
    {
        private const string CategoryLinqReverse = "Linq.Reverse";

        [TestMethod]
        [TestCategory(CategoryLinqReverse)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateSource()
        {
            var source = default(IAsyncEnumerable<string>);
            var query = source.Reverse();
        }

        [TestMethod]
        [TestCategory(CategoryLinqReverse)]
        public async Task ArraysAreBuffered()
        {
            // A sneaky implementation may try to optimize for the case where the collection
            // implements IList or (even more "reliable") is an array: it mustn’t do this,
            // as otherwise the results can be tainted by side-effects within iteration
            var source = new [] { 0, 1, 2, 3 };
            var adapter = source.AsAsyncEnumerable();

            var query = adapter.Reverse();
            source[1] = 99; // This change *will* be seen due to deferred execution
            using (var iterator = query.GetAsyncEnumerator())
            {
                await iterator.MoveNextAsync();
                Assert.AreEqual(3, iterator.Current);

                source[2] = 100; // This change *won’t* be seen               
                await iterator.MoveNextAsync();
                Assert.AreEqual(2, iterator.Current);

                await iterator.MoveNextAsync();
                Assert.AreEqual(99, iterator.Current);

                await iterator.MoveNextAsync();
                Assert.AreEqual(0, iterator.Current);
            }
        }
    }
}
