using System;
using System.Linq;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Enterprise.Core.Reactive.Linq.AsyncObservable;

namespace Enterprise.Tests.Reactive.Select
{
    [TestClass]
    public class SelectTest
    {
        private const string CategoryReactiveSelect = "Reactive.Select";

        [TestMethod]
        [TestCategory(CategoryReactiveSelect)]
        public async Task SimpleProjectionToDifferentType()
        {
            var source = new[] { 1, 5, 2 }.ToAsyncObservable();
            var result = source.Select(x => x.ToString());
            Assert.IsTrue(await result.SequenceEqual(new[] { "1", "5", "2" }));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveSelect)]
        public async Task SideEffectsInProjection()
        {
            var source = new[] { 0, 0, 0 }.ToAsyncObservable();
            var count = 0;
            var query = source.Select(x => count++);
            Assert.IsTrue(await query.SequenceEqual(new[] { 0, 1, 2 }));
            Assert.IsTrue(await query.SequenceEqual(new[] { 3, 4, 5 }));
            count = 10;
            Assert.IsTrue(await query.SequenceEqual(new[] { 10, 11, 12 }));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveSelect)]
        public async Task WhereAndSelect()
        {
            var iteration = 0;
            var source = Create<int>((yield, cancellationToken) =>
            {
                iteration++;

                return yield.ReturnAllAsync(new[] { 1, 3, 4, 2, 8, 1 }, cancellationToken);
            });

            var result = from x in source
                         where x < 4
                         select x * 2;

            Assert.IsTrue(await result.SequenceEqual(new[] { 2, 6, 4, 2 }));
            Assert.AreEqual(1, iteration);

            await result;
            Assert.AreEqual(2, iteration);
        }

        [TestMethod]
        [TestCategory(CategoryReactiveSelect)]
        public async Task AsyncProjection()
        {
            Func<int, Task<int>> selectorAsync = async item =>
            {
                await Task.Delay(1);

                return item * 2;
            };

            var source = new[] { 1, 2, 3 }.ToAsyncObservable();
            var query = from item in source where item > 0 select selectorAsync(item);

            var result = query.Concat();

            Assert.IsTrue(await result.SequenceEqual(new[] { 2, 4, 6 }));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveSelect)]
        public async Task AsyncProjectionParallel()
        {
            var source = new[] { "A", "B", "C" }.ToAsyncObservable();
            Func<string, Task> selectorAsync = Console.Out.WriteLineAsync;

            var query = await (
                from item in source
                where item.Length > 0
                select selectorAsync(item))
                .AsAsyncEnumerable().ToArrayAsync();

            await Task.WhenAll(query);

            Assert.IsTrue(query.All(t => t.IsCompleted));
        }
    }
}
