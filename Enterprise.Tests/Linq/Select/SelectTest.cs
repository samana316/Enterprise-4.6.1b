﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Tests.Linq.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enterprise.Tests.Linq.Select
{
    [TestClass]
    public class SelectTest
    {
        private const string CategoryLinqSelect = "Linq.Select";

        [TestMethod]
        [TestCategory(CategoryLinqSelect)]
        public async Task SimpleProjectionToDifferentType()
        {
            var source = new RealAsyncEnumerable<int>(1, 5, 2);
            var result = source.Select(x => x.ToString());
            Assert.IsTrue(await result.SequenceEqualAsync(new[] { "1", "5", "2" }));
        }

        [TestMethod]
        [TestCategory(CategoryLinqSelect)]
        public async Task SideEffectsInProjection()
        {
            var source = new RealAsyncEnumerable<int>(0, 0, 0);
            var count = 0;
            var query = source.Select(x => count++);
            Assert.IsTrue(await query.SequenceEqualAsync(new[] { 0, 1, 2 }));
            Assert.IsTrue(await query.SequenceEqualAsync(new[] { 3, 4, 5 }));
            count = 10;
            Assert.IsTrue(await query.SequenceEqualAsync(new[] { 10, 11, 12 }));
        }

        [TestMethod]
        [TestCategory(CategoryLinqSelect)]
        public async Task WhereAndSelect()
        {
            var source = new RealAsyncEnumerable<int>(1, 3, 4, 2, 8, 1 );
            var result = from x in source
                         where x < 4
                         select x * 2;
            Assert.IsTrue(await result.SequenceEqualAsync(new[] { 2, 6, 4, 2 }));
        }

        [TestMethod]
        [TestCategory(CategoryLinqSelect)]
        public async Task AsyncProjection()
        {
            Func<int, Task<int>> selectorAsync = async item =>
            {
                await Task.Delay(1);

                return item * 2;
            };

            var source = new RealAsyncEnumerable<int>(1, 2, 3);
            var query = from item in source select selectorAsync(item);

            var result = AsyncEnumerable.Create<int>((y, cancellationToken) =>
            {
                return query.ForEachAsync(async (item, cancellationToken2) => 
                {
                    await y.ReturnAsync(await item, cancellationToken2);
                }, cancellationToken);
            });

            Assert.IsTrue(await result.SequenceEqualAsync(new[] { 2, 4, 6 }));
        }

        [TestMethod]
        [TestCategory(CategoryLinqSelect)]
        public async Task AsyncProjectionParallel()
        {
            var source = new RealAsyncEnumerable<string>("A", "B", "C");
            Func<string, Task> selectorAsync = Console.Out.WriteLineAsync;

            var query =  await (from item in source select selectorAsync(item)).ToArrayAsync();
            await Task.WhenAll(query);

            Assert.IsTrue(query.All(t => t.IsCompleted));
        }
    }
}
