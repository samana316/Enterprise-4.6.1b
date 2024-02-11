using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Tests.Linq.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Enterprise.Core.Linq.AsyncEnumerable;

namespace Enterprise.Tests.Linq.Join
{
    [TestClass]
    public sealed class JoinTest
    {
        private const string CategoryLinqJoin = "Linq.Join";

        [TestMethod]
        [TestCategory(CategoryLinqJoin)]
        public void ExecutionIsDeferred()
        {
            var outer = new ThrowAsyncEnumerable<int>();
            var inner = new ThrowAsyncEnumerable<string>();

            var query = from x in outer
                        join y in inner on x equals y.Length
                        select x + ":" + y;
        }

        [TestMethod]
        [TestCategory(CategoryLinqJoin)]
        public async Task DifferentSourceTypes()
        {
            var outer = new RealAsyncEnumerable<int>(5, 3, 7);
            var inner = new RealAsyncEnumerable<string>("bee", "giraffe", "tiger", "badger", "ox", "cat", "dog");

            var query = from x in outer
                        join y in inner on x equals y.Length
                        select x + ":" + y;

            var expected = from x in await outer.ToArrayAsync()
                           join y in inner.ToArray() on x equals y.Length
                           select x + ":" + y;

            Assert.IsTrue(await query.SequenceEqualAsync(expected));
        }

        [TestMethod]
        [TestCategory(CategoryLinqJoin)]
        public async Task OuterSequenceIsStreamed()
        {
            var spy = new List<int>();
            var outer = Create<int>(async (yield, cancellationToken) => 
            {
                var source = new[] { 5, 3, 7 };

                foreach (var item in source)
                {
                    spy.Add(item);
                    await yield.ReturnAsync(item, cancellationToken);
                }
            });

            var inner = new[] { "bee", "giraffe", "tiger", "badger", "ox", "cat", "dog" };

            var query = from x in outer
                        join y in inner on x equals y.Length
                        select x + ":" + y;

            using (var iterator = query.GetAsyncEnumerator())
            {
                await iterator.MoveNextAsync();
                Assert.AreEqual(1, spy.Count);
                Assert.AreEqual("5:tiger", iterator.Current);

                await iterator.MoveNextAsync();
                Assert.AreEqual(2, spy.Count);
                Assert.AreEqual("3:bee", iterator.Current);
            }
        }
    }
}
