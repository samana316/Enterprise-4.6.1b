using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Core.Reactive;
using Enterprise.Core.Reactive.Linq;
using Enterprise.Tests.Reactive.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enterprise.Tests.Poc
{
    [TestClass]
    public sealed class AsyncObservableTest
    {
        private const string CategoryAsyncObservable = "Poc.AsyncObservable";

        [TestMethod]
        [TestCategory(CategoryAsyncObservable)]
        public async Task SubscribeAsync()
        {
            var source = AsyncObservable.Create<int>(async (yield, cancellationToken) =>
            {
                await yield.ReturnAsync(1, cancellationToken);
                await yield.ReturnAsync(2, cancellationToken);
                await yield.ReturnAsync(3, cancellationToken);
            });

            var observer = new SpyAsyncObserver<int>();

            await source.SubscribeAsync(observer, CancellationToken.None);

            Assert.IsTrue(await observer.Items.SequenceEqualAsync(new[] { 1, 2, 3 }));
        }

        [TestMethod]
        [TestCategory(CategoryAsyncObservable)]
        [Timeout(30000)]
        public async Task TempForEachBreak()
        {
            var source = AsyncObservable.Create<int>(async (yield, cancellationToken) =>
            {
                var i = 0;
                while (true)
                {
                    i++;

                    if (i > 3)
                    {
                        yield.Break();
                    }

                    await yield.ReturnAsync(i, cancellationToken);
                }
            });

            var query = source.Take(2);

            var observer = new SpyAsyncObserver<int>();
            await source.ForEachAsync(observer.OnNextAsync, CancellationToken.None);

            observer.Reset();
            await query.ForEachAsync(observer.OnNextAsync, CancellationToken.None);
        }

        [TestMethod]
        [TestCategory(CategoryAsyncObservable)]
        public async Task AsyncYieldMixTest()
        {
            var source = AsyncObservable.Interval(TimeSpan.FromMilliseconds(10)).Take(3);
            var enumerable = this.AsyncYieldMix(source, CancellationToken.None);

            foreach (var item in enumerable)
            {
                Trace.WriteLine(item);
            }

            await Task.Yield();
        }

        private IEnumerable<T> AsyncYieldMix<T>(
            IAsyncObservable<T> source,
            CancellationToken cancellationToken)
        {
            using (var iterator = source.GetAsyncEnumerator())
            {
                while (iterator.MoveNextAsync(cancellationToken).Result)
                {
                    yield return iterator.Current;
                }
            }

            yield break;
        }
    }
}
