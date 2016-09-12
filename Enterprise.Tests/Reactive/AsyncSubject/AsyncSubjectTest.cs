using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Core.Reactive;
using Enterprise.Core.Reactive.Linq;
using Enterprise.Tests.Reactive.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enterprise.Tests.Reactive.Subject
{
    [TestClass]
    public sealed class AsyncSubjectTest
    {
        private const int DefaultTimeout = 1000;

        private const string CategoryReactiveAsyncSubject = "Reactive.AsyncSubject";

        [TestMethod]
        [TestCategory(CategoryReactiveAsyncSubject)]
        [Timeout(DefaultTimeout)]
        public async Task Subscribe()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var source =  AsyncSubject.Create<int>(async (yield, cancellationToken) =>
            {
                var i = 0;
                while (true)
                {
                    i++;
                    await yield.ReturnAsync(i, cancellationToken);
                    await Task.Delay(10);
                }
            });

            var observer1 = new SpyAsyncObserver<int>();
            var observer2 = new SpyAsyncObserver<int>();

            var subscription1 = source.Subscribe(observer1);
            var task = source.RunAsync(cancellationTokenSource.Token);
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

            try
            {
                cancellationTokenSource.Cancel();
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }
        }

        [TestMethod]
        [TestCategory(CategoryReactiveAsyncSubject)]
        [Timeout(DefaultTimeout)]
        public async Task SubscribeAsync()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            var source = AsyncSubject.Create<int>(async (yield, cancellationToken) =>
            {
                var i = 0;
                while (true)
                {
                    i++;
                    await yield.ReturnAsync(i, cancellationToken);
                    await Task.Delay(10);
                }
            });

            var subject =
                (from item in source
                 where item % 2 != 0
                 select item).AsAsyncSubject();

            var observer1 = new SpyAsyncObserver<int>();
            var observer2 = new SpyAsyncObserver<int>();

            var subscription1 = subject.SubscribeAsync(observer1);
            var task = subject.RunAsync(cancellationTokenSource.Token);
            await Task.Delay(25);

            var subscription2 = subject.SubscribeAsync(observer2);
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

            try
            {
                cancellationTokenSource.Cancel();
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }
        }
    }
}
