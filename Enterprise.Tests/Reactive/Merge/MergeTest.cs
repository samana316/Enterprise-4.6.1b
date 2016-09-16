using System;
using System.Linq;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Core.Reactive;
using Enterprise.Core.Reactive.Linq;
using Enterprise.Tests.Reactive.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Enterprise.Core.Reactive.Linq.AsyncObservable;

namespace Enterprise.Tests.Reactive.Merge
{
    [TestClass]
    public class MergeTest
    {
        private const int DefaultTimeout = 1000;

        private const string CategoryReactiveMerge = "Reactive.Merge";

        [TestMethod]
        [TestCategory(CategoryReactiveMerge)]
        public async Task Simple()
        {
            var s1 = AsyncObservable.Interval(TimeSpan.FromMilliseconds(250))
                .Take(3);
            
            var s2 = AsyncObservable.Interval(TimeSpan.FromMilliseconds(150))
                .Take(5)
                .Select(i => i + 100);

            var observer1 = new SpyAsyncObserver<long>();
            var observer2 = new SpyAsyncObserver<long>();

            var query = s1.Merge(s2);
            var observer = new SpyAsyncObserver<long>();
            await query.SubscribeAsync(observer);

            // Can't compare the entire sequence due to race condition.
            var expected = new long[] { 100, 0, 101 };
            Assert.IsTrue(await observer.Items.Take(3).SequenceEqualAsync(expected));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveMerge)]
        public async Task Observables()
        {
            var sources = Create<IAsyncObservable<long>>((yield, cancellationToken) => 
            {
                var yields =
                    from dueTime in new[] { 300, 200, 100 }
                    let timer = AsyncObservable.Timer(TimeSpan.FromMilliseconds(dueTime))
                        .Select(i => i + dueTime)
                    select yield.ReturnAsync(timer, cancellationToken);

                return Task.WhenAll(yields);
            });

            var query = sources.Merge();
            var observer = new SpyAsyncObserver<long>();
            await query.SubscribeAsync(observer);

            var expected = new long[] { 100, 200, 300 };
            Assert.IsTrue(await observer.Items.SequenceEqualAsync(expected));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveMerge)]
        public async Task Tasks()
        {
            var sources = Create<Task<int>>((yield, cancellationToken) =>
            {
                var tasks =
                    from dueTime in new int[] { 300, 200, 100 }
                    select Task.Delay(dueTime, cancellationToken).ContinueWith(async t => 
                    {
                        await t;

                        return dueTime;
                    }).Unwrap();

                return yield.ReturnAllAsync(tasks, cancellationToken);
            });

            var query = sources.Merge();
            var observer = new SpyAsyncObserver<int>();
            await query.SubscribeAsync(observer);

            var expected = new[] { 100, 200, 300 };
            Assert.IsTrue(await observer.Items.SequenceEqualAsync(expected));
        }
    }
}
