using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Tests.Linq.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Enterprise.Core.Linq.AsyncEnumerable;

namespace Enterprise.Tests.Linq.SelectMany
{
    [TestClass]
    public sealed class SelectManyTest
    {
        private const string CategoryLinqSelectMany = "Linq.SelectMany";

        [TestMethod]
        [TestCategory(CategoryLinqSelectMany)]
        public async Task FlattenWithProjection()
        {
            var source = Create<IEnumerable<int>>(async (y, ct) =>
            {
                await y.ReturnAsync(new[] { 1, 2 }, ct);
                await y.ReturnAsync(Enumerable.Range(3, 2), ct);
                await y.ReturnAsync(AsyncEnumerable.Range(5, 2), ct);
                await y.ReturnAsync(new RealAsyncEnumerable<int>(7, 8), ct);
            });

            var result = source.SelectMany(x => x);
            var expected = AsyncEnumerable.Range(1, 8);
            Assert.IsTrue(await result.SequenceEqualAsync(expected));
        }

        [TestMethod]
        [TestCategory(CategoryLinqSelectMany)]
        public async Task FlattenWithProjectionQuery()
        {
            var source = Create<IEnumerable<int>>(async (y, ct) => 
            {
                await y.ReturnAsync(new[] { 1, 2 }, ct);
                await y.ReturnAsync(Enumerable.Range(3, 2), ct);
                await y.ReturnAsync(AsyncEnumerable.Range(5, 2), ct);
                await y.ReturnAsync(new RealAsyncEnumerable<int>(7, 8), ct);
            });

            var result =
                from item in source
                from collectionItem in item
                select collectionItem;

            var expected = AsyncEnumerable.Range(1, 8);
            Assert.IsTrue(await result.SequenceEqualAsync(expected));
        }

        [TestMethod]
        [TestCategory(CategoryLinqSelectMany)]
        public async Task FlattenWithProjectionAndIndex()
        {
            var numbers = new RealAsyncEnumerable<int>(3, 5, 20, 15);

            var query = numbers.SelectMany(
                (x, index) => (x + index).ToString(),
                (x, c) => x + ": " + c);

            Assert.IsTrue(await query.SequenceEqualAsync(
                new[] { "3: 3", "5: 6", "20: 2", "20: 2", "15: 1", "15: 8" }));
        }

        [TestMethod]
        [TestCategory(CategoryLinqSelectMany)]
        public async Task ExecutionIsDeferred()
        {
            var spy = new List<int>();

            var source = Create<IAsyncEnumerable<int>>(async (y1, ct1) => 
            {
                spy.Clear();
                for (var i = 1; i < 10; i++)
                {
                    spy.Add(i);
                    var child = Create<int>(async (y2, ct2) => 
                    {
                        for (var j = 11; j < 20; j++)
                        {
                            spy.Add(j);
                            await y2.ReturnAsync(j, ct2);
                        }
                    });

                    await y1.ReturnAsync(child, ct1);
                }
            });

            using (var enumerator = source.GetAsyncEnumerator())
            {
                await enumerator.MoveNextAsync();
                Trace.WriteLine(enumerator.Current);

                await enumerator.MoveNextAsync();
                Trace.WriteLine(enumerator.Current);
            }

            Assert.AreEqual(2, spy.Count);
            spy.Clear();

            var query =
                from item in source
                from collectionItem in item
                select collectionItem;

            Assert.AreEqual(0, spy.Count);
            spy.Clear();

            using (var enumerator = query.GetAsyncEnumerator())
            {
                await enumerator.MoveNextAsync();
                Trace.WriteLine(enumerator.Current);
            }

            Assert.AreEqual(2, spy.Count);
        }
    }
}
