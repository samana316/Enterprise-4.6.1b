using System;
using System.Linq;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Core.Reactive.Linq;
using Enterprise.Tests.Reactive.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Enterprise.Core.Reactive.Linq.AsyncObservable;

namespace Enterprise.Tests.Reactive.Sample
{
    [TestClass]
    public class SampleTest
    {
        private const int DefaultTimeout = 1000;

        private const string CategoryReactiveSample = "Reactive.Sample";

        [TestMethod]
        [TestCategory(CategoryReactiveSample)]
        public async Task Interval()
        {
            var source = AsyncObservable.Interval(TimeSpan.FromMilliseconds(15));
            var query = source.Sample(TimeSpan.FromMilliseconds(100)).Take(3);

            var observer = new SpyAsyncObserver<long>();
            await query.SubscribeAsync(observer);

            var items = await observer.Items.ToListAsync();
            Assert.IsTrue(items[0] >= 5 && items[0] <= 10);
            Assert.IsTrue(items[1] >= 10 && items[1] <= 15);
            Assert.IsTrue(items[2] >= 15 && items[2] <= 20);
        }

        [TestMethod]
        [TestCategory(CategoryReactiveSample)]
        [Timeout(DefaultTimeout)]
        public async Task Sampler()
        {
            var source = AsyncObservable.Interval(TimeSpan.FromMilliseconds(15));
            var sampler = AsyncObservable.Interval(TimeSpan.FromMilliseconds(100));
            var query = source.Sample(sampler).Take(3);

            var observer = new SpyAsyncObserver<long>();
            await query.SubscribeAsync(observer);

            var items = await observer.Items.ToListAsync();
            Assert.IsTrue(items[0] >= 5 && items[0] <= 10);
            Assert.IsTrue(items[1] >= 10 && items[1] <= 15);
            Assert.IsTrue(items[2] >= 15 && items[2] <= 20);
        }
    }
}
