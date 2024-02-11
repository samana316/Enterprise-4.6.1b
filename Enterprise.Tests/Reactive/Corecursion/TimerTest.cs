using System;
using System.Linq;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Core.Reactive.Linq;
using Enterprise.Tests.Reactive.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enterprise.Tests.Reactive.Timer
{
    [TestClass]
    public sealed class TimerTest
    {
        private const int DefaultTimeout = 1000;

        private const string CategoryReactiveTimer = "Reactive.Timer";

        [TestMethod]
        [TestCategory(CategoryReactiveTimer)]
        [Timeout(DefaultTimeout)]
        public async Task Simple()
        {
            var source = AsyncObservable.Timer(TimeSpan.FromMilliseconds(10));

            var observer = new SpyAsyncObserver<long>();
            using (source.SubscribeAsync(observer))
            {
                await Task.Delay(20);
            }

            await Task.Delay(20);

            Assert.AreEqual(0, await observer.Items.SingleAsync());
            Assert.IsTrue(observer.IsCompleted);
            Assert.IsFalse(observer.Error.InnerExceptions.Any());
        }

        [TestMethod]
        [TestCategory(CategoryReactiveTimer)]
        [Timeout(DefaultTimeout)]
        public async Task WithPeriod()
        {
            var source = AsyncObservable.Timer(
                DateTimeOffset.Now.AddMilliseconds(20),
                TimeSpan.FromMilliseconds(10));

            var observer = new SpyAsyncObserver<long>();
            var count = 0;

            using (source.SubscribeAsync(observer))
            {
                await Task.Delay(100);
                count = await observer.Items.CountAsync();

                Assert.IsTrue(count > 0);
                Assert.IsTrue(count < 10);
                Assert.IsFalse(observer.IsCompleted);
                Assert.IsFalse(observer.Error.InnerExceptions.Any());
            }

            await Task.Delay(50);
            var count2 = await observer.Items.CountAsync();

            Assert.AreEqual(count, count2);
            Assert.IsTrue(observer.IsCompleted);
            Assert.IsTrue(observer.Error.InnerExceptions.Any());
        }
    }
}
