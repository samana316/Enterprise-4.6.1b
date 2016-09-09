using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Tests.Reactive.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Enterprise.Core.Reactive.Linq.AsyncObservable;

namespace Enterprise.Tests.Reactive.Compatibility
{
    [TestClass]
    public sealed class CompatibilityTest
    {
        private const int DefaultTimeout = 1000;

        private const string CategoryReactiveCompatibility = "Reactive.Compatibility";

        [TestMethod]
        [TestCategory(CategoryReactiveCompatibility)]
        [Timeout(DefaultTimeout)]
        public async Task Subscribe()
        {
            var source = Create<int>(async (yield, CompatibilityToken) =>
            {
                var i = 0;
                while (true)
                {
                    i++;
                    await yield.ReturnAsync(i, CompatibilityToken);
                    await Task.Delay(10);
                }
            });

            source =
                from item in source
                where item % 2 != 0
                select item;

            var observer1 = new SpyAsyncObserver<int>();
            var observer2 = new SpyAsyncObserver<int>();

            var subscription1 = source.Subscribe(observer1);
            await Task.Delay(25);

            var subscription2 = source.Subscribe(observer2);
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

        [TestMethod]
        [TestCategory(CategoryReactiveCompatibility)]
        [Timeout(DefaultTimeout)]
        public async Task SubscribeAsync()
        {
            var source = new TestObservable();

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

        private sealed class TestObservable : IObservable<int>
        {
            public IDisposable Subscribe(
                IObserver<int> observer)
            {
                return this.SubscribeAsync(observer);
            }

            private async Task SubscribeAsync(
                IObserver<int> observer)
            {
                var i = 0;
                while (true)
                {
                    i++;
                    observer.OnNext(i);
                    await Task.Delay(10);
                }
            }
        }
    }
}
