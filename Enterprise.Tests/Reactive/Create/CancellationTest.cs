using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Tests.Reactive.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Enterprise.Core.Reactive.Linq.AsyncObservable;

namespace Enterprise.Tests.Reactive.Cancellation
{
    [TestClass]
    public sealed class CancellationTest
    {
        private const int DefaultTimeout = 1000;

        private const string CategoryReactiveCancellation = "Reactive.Cancellation";

        [TestMethod]
        [TestCategory(CategoryReactiveCancellation)]
        [Timeout(DefaultTimeout)]
        public async Task CancelWithToken()
        {
            var source = Create<int>(async (yield, cancellationToken) => 
            {
                var i = 0;
                while (true)
                {
                    i++;
                    await yield.ReturnAsync(i, cancellationToken);
                    await Task.Delay(10);
                }
            });

            var observer = new SpyAsyncObserver<int>();

            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                using (var subscription = source.SubscribeAsync(observer, cancellationTokenSource.Token))
                {
                    await Task.Delay(50);

                    cancellationTokenSource.Cancel();

                    await subscription;
                }
            }

            Assert.IsTrue(await observer.Items.CountAsync() > 0);
            Assert.IsTrue(observer.Error.InnerExceptions.Any());
        }

        [TestMethod]
        [TestCategory(CategoryReactiveCancellation)]
        [Timeout(DefaultTimeout)]
        public async Task CancelWithUnsubscriber()
        {
            var source = Create<int>(async (yield, cancellationToken) =>
            {
                var i = 0;
                while (true)
                {
                    i++;
                    await yield.ReturnAsync(i, cancellationToken);
                    await Task.Delay(10);
                }
            });

            var observer = new SpyAsyncObserver<int>();

            var subscription = source.SubscribeAsync(observer);
            await Task.Delay(50);

            subscription.Dispose();

            Assert.IsTrue(await observer.Items.CountAsync() > 0);
            Assert.IsFalse(observer.Error.InnerExceptions.Any());
        }

        [TestMethod]
        [TestCategory(CategoryReactiveCancellation)]
        [Timeout(DefaultTimeout)]
        public async Task MultipleSubscriptions()
        {
            var source = Create<int>(async (yield, cancellationToken) =>
            {
                var i = 0;
                while (true)
                {
                    i++;
                    await yield.ReturnAsync(i, cancellationToken);
                    await Task.Delay(10);
                }
            });

            source =
                from item in source
                where item % 2 != 0
                select item;

            var observer1 = new SpyAsyncObserver<int>();
            var observer2 = new SpyAsyncObserver<int>();

            var subscription1 = source.SubscribeAsync(observer1);
            await Task.Delay(25);

            var subscription2 = source.SubscribeAsync(observer2);
            await Task.Delay(50);

            subscription1.Dispose();
            await Task.Delay(100);
            subscription2.Dispose();

            var count1 = await observer1.Items.CountAsync();
            Trace.WriteLine(count1, "Count1");

            var count2 = await observer2.Items.CountAsync();
            Trace.WriteLine(count2, "Count2");

            Assert.IsTrue(count1 > 0);
            Assert.IsFalse(observer1.Error.InnerExceptions.Any());

            Assert.IsTrue(count2 > count1);
            Assert.IsFalse(observer2.Error.InnerExceptions.Any());
        }
    }
}
