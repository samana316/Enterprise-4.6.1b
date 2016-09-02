using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enterprise.Tests.Poc
{
    [TestClass]
    public sealed class IteratorObserverDualityTest
    {
        private const string CategoryIteratorObserverDuality = "Poc.IteratorObserverDuality";

        [TestMethod]
        [TestCategory(CategoryIteratorObserverDuality)]
        public async Task SubscribeAsync()
        {
            var source = AsyncEnumerable.Range(1, 5);

            await source.SubscribeAsync(new TestObserver<int>(), CancellationToken.None);
        }

        private sealed class TestObserver<T> : IObserver<T>
        {
            public void OnCompleted()
            {
                Trace.WriteLine("OnCompleted");
            }

            public void OnError(
                Exception error)
            {
                Trace.WriteLine(error, "OnError");
            }

            public void OnNext(
                T value)
            {
                Trace.WriteLine(value, "OnNext");
            }
        }
    }

    internal static class AsyncEnumerableExtensions
    {
        public static async Task SubscribeAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            IObserver<TSource> observer,
            CancellationToken cancellationToken)
        {
            try
            {
                await source.ForEachAsync(observer.OnNext, cancellationToken);
            }
            catch (Exception exception)
            {
                observer.OnError(exception);
            }
            finally
            {
                observer.OnCompleted();
            }
        }
    }
}
