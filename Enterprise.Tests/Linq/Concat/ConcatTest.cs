using System;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Tests.Linq.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enterprise.Tests.Linq.Concat
{
    [TestClass]
    public class ConcatTest
    {
        private const int DefaultTimeout = 1000;

        private const string CategoryLinqConcat = "Linq.Concat";

        [TestMethod]
        [TestCategory(CategoryLinqConcat)]
        [ExpectedException(typeof(NotImplementedException))]
        public async Task FirstSequenceIsntAccessedBeforeFirstUse()
        {
            var first = new ThrowAsyncEnumerable<int>();
            var second = new[] { 5 };

            var query = first.Concat(second);

            using (var iterator = query.GetAsyncEnumerator())
            {
                await iterator.MoveNextAsync();
            }
        }

        [TestMethod]
        [TestCategory(CategoryLinqConcat)]
        [ExpectedException(typeof(NotImplementedException))]
        public async Task SecondSequenceIsntAccessedBeforeFirstUse()
        {
            var first = new RealAsyncEnumerable<int>(5);
            var second = new ThrowAsyncEnumerable<int>();

            var query = first.Concat(second);

            using (var iterator = query.GetAsyncEnumerator())
            {
                Assert.IsTrue(await iterator.MoveNextAsync());
                Assert.AreEqual(5, iterator.Current);

                await iterator.MoveNextAsync();
            }
        }

        [TestMethod]
        [TestCategory(CategoryLinqConcat)]
        public async Task Simple()
        {
            var first = new RealAsyncEnumerable<int>(1, 2, 3);
            var second = new RealAsyncEnumerable<int>(4, 5);

            var query = first.Concat(second);
            var expected = AsyncEnumerable.Range(1, 5);

            Assert.IsTrue(await query.SequenceEqualAsync(expected));
            Assert.IsTrue(await query.SequenceEqualAsync(expected));
        }
    }
}
