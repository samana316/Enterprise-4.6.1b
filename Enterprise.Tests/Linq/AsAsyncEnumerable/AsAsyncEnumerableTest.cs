using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enterprise.Tests.Linq.AsAsyncEnumerable
{
    [TestClass]
    public class AsAsyncEnumerableTest
    {
        private const int DefaultTimeout = 1000;
        private const string CategoryLinqAsAsyncEnumerable = "Linq.AsAsyncEnumerable";

        [TestMethod]
        [TestCategory(CategoryLinqAsAsyncEnumerable)]
        public async Task AnonymousType()
        {
            var source = new[] {
                new { FirstName = "Clark", LastName = "Kent" },
                new { FirstName = "Bruce", LastName = "Wayne" }
            }.ToList();

            var result = source.AsAsyncEnumerable();

            var query =
                from item in result
                select item.FirstName;

            Assert.IsTrue(await query.SequenceEqualAsync(new[] { "Clark", "Bruce" }));
        }

        [TestMethod]
        [TestCategory(CategoryLinqAsAsyncEnumerable)]
        public async Task CollectionType()
        {
            var source = new List<int>(new[] { 1, 2, 3 });
            var result = source.AsAsyncEnumerable();

            Assert.IsTrue(await result.SequenceEqualAsync(source));
            Assert.IsTrue(result is IList<int>);
            Assert.IsTrue(result is IReadOnlyList<int>);
            Assert.IsTrue(result is ICollection<int>);
            Assert.IsTrue(result is IReadOnlyCollection<int>);

            var result2 = (source as IList).AsAsyncEnumerable();
            Assert.IsTrue(result2 is IList);
            Assert.IsTrue(result2 is ICollection);

            source.Add(4);
            Assert.IsTrue(await result.SequenceEqualAsync(source));
        }

        [TestMethod]
        [TestCategory(CategoryLinqAsAsyncEnumerable)]
        public async Task CollectionTypeUnsafe()
        {
            var source = Array.CreateInstance(typeof(int), new[] { 5 }, new[] { -5 });
            var result = source.AsAsyncEnumerable();

            Assert.IsTrue(result is IList);
            Assert.IsTrue(result is ICollection);

            await result.ForEachAsync(x => Trace.WriteLine(x, "MoveNextAsync"));
        }

        [TestMethod]
        [TestCategory(CategoryLinqAsAsyncEnumerable)]
        public async Task InfiniteTimeout()
        {
            var count = 0L;
            var source = this.InfiniteIterator(0);
            var result = source.AsAsyncEnumerable();

            try
            {
                using (var cancellationTokenSource = new CancellationTokenSource(DefaultTimeout))
                {
                    using (var enumerator = result.GetAsyncEnumerator())
                    {
                        while (await enumerator.MoveNextAsync(cancellationTokenSource.Token))
                        {
                            count++;
                        }
                    }
                }
            }
            catch (OperationCanceledException exception)
            {
                Trace.WriteLine(exception);
            }

            Trace.WriteLine(count);
            Assert.IsTrue(count > 0);
        }

        private IEnumerable<T> InfiniteIterator<T>(
            T value)
        {
            while (true)
            {
                Task.Delay(100).Wait();
                yield return value;
            }
        }
    }
}
