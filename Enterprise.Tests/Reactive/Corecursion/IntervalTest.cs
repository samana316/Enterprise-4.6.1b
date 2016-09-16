using System;
using System.Linq;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Core.Reactive.Linq;
using Enterprise.Tests.Reactive.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enterprise.Tests.Reactive.Interval
{
    [TestClass]
    public sealed class IntervalTest
    {
        private const int DefaultTimeout = 1000;

        private const string CategoryReactiveInterval = "Reactive.Interval";

        [TestMethod]
        [TestCategory(CategoryReactiveInterval)]
        [Timeout(DefaultTimeout)]
        public async Task Simple()
        {
            var source = AsyncObservable.Interval(TimeSpan.FromMilliseconds(10));

            var observer = new SpyAsyncObserver<long>();
            using (source.SubscribeAsync(observer))
            {
                await Task.Delay(50);

                Assert.IsTrue(await observer.Items.AnyAsync());
                Assert.IsTrue(await observer.Items.CountAsync() < 10);
                Assert.IsFalse(observer.IsCompleted);
                Assert.IsFalse(observer.Error.InnerExceptions.Any());
            }

            await Task.Delay(50);

            Assert.IsTrue(await observer.Items.AnyAsync());
            Assert.IsTrue(await observer.Items.CountAsync() < 10);
            Assert.IsTrue(observer.IsCompleted);
            Assert.IsTrue(observer.Error.InnerExceptions.Any());
        }

        [TestMethod]
        [TestCategory(CategoryReactiveInterval)]
        [Timeout(DefaultTimeout)]
        public async Task Query()
        {
            var source = AsyncObservable.Interval(TimeSpan.FromMilliseconds(5));
            var query = source.Where(x => x % 2 != 0).Take(5);
            var observer = new SpyAsyncObserver<long>();

            await query.SubscribeAsync(observer);

            Assert.IsTrue(await observer.Items.SequenceEqualAsync(new long[] { 1, 3, 5, 7, 9 }));
        }
    }
}
