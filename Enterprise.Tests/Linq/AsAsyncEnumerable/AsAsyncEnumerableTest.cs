using System;
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
