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
    public sealed class GroupJoinTest
    {
        private const string CategoryLinqGroupJoin = "Linq.GroupJoin";

        [TestMethod]
        [TestCategory(CategoryLinqGroupJoin)]
        public void ExecutionIsDeferred()
        {
            var outer = new ThrowAsyncEnumerable<int>();
            var inner = new ThrowAsyncEnumerable<string>();

            var query = from x in outer
                        join y in inner on x equals y.Length into matches
                        select x + ":" + matches;
        }

        [TestMethod]
        [TestCategory(CategoryLinqGroupJoin)]
        public async Task DifferentSourceTypes()
        {
            var outer = new RealAsyncEnumerable<int>(5, 3, 4, 7);
            var inner = new RealAsyncEnumerable<string>("bee", "giraffe", "tiger", "badger", "ox", "cat", "dog");

            var query = from x in outer
                        join y in inner on x equals y.Length into matches
                        from z in matches.DefaultIfEmpty("null")
                        select x + ":" + z;

            var expected = from x in await outer.ToArrayAsync()
                           join y in inner.ToArray() on x equals y.Length into matches
                           from z in matches.DefaultIfEmpty("null")
                           select x + ":" + z;

            Assert.IsTrue(await query.SequenceEqualAsync(expected));
        }

        [TestMethod]
        [TestCategory(CategoryLinqGroupJoin)]
        public async Task OuterSequenceIsStreamed()
        {
            var spy = new List<int>();
            var outer = Create<int>(async (yield, cancellationToken) =>
            {
                var source = new[] { 5, 3, 4, 7 };

                foreach (var item in source)
                {
                    spy.Add(item);
                    await yield.ReturnAsync(item, cancellationToken);
                }
            });

            var inner = new[] { "bee", "giraffe", "tiger", "badger", "ox", "cat", "dog" };

            var query = from x in outer
                        join y in inner on x equals y.Length into matches
                        from z in matches.DefaultIfEmpty("null")
                        select x + ":" + z;

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
