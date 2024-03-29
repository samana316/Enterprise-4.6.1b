﻿using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Core.Reactive.Linq;
using Enterprise.Tests.Reactive.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enterprise.Tests.Reactive.Subject
{
    [TestClass]
    public sealed class PublishTest
    {
        private const int DefaultTimeout = 1000;

        private const string CategoryReactivePublish = "Reactive.Publish";

        [TestMethod]
        [TestCategory(CategoryReactivePublish)]
        [Timeout(DefaultTimeout)]
        public async Task Subscribe()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var source =  AsyncObservable.Create<int>(async (yield, cancellationToken) =>
            {
                var i = 0;
                while (true)
                {
                    i++;
                    await yield.ReturnAsync(i, cancellationToken);
                    await Task.Delay(10);
                }
            }).Publish();

            var observer1 = new SpyAsyncObserver<int>();
            var observer2 = new SpyAsyncObserver<int>();

            var subscription1 = source.Subscribe(observer1);
            var task = source.ConnectAsync(cancellationTokenSource.Token);
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
        [TestCategory(CategoryReactivePublish)]
        [Timeout(DefaultTimeout)]
        public async Task SubscribeAsync()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            var source = AsyncObservable.Create<int>(async (yield, cancellationToken) =>
            {
                var i = 0;
                while (true)
                {
                    i++;
                    await yield.ReturnAsync(i, cancellationToken);
                    await Task.Delay(10);
                }
            }).Publish();

            var result =
                (from item in source
                 where item % 2 != 0
                 select item).Publish();

            var observer1 = new SpyAsyncObserver<int>();
            var observer2 = new SpyAsyncObserver<int>();

            var subscription1 = result.SubscribeAsync(observer1);
            var task = result.ConnectAsync(cancellationTokenSource.Token);
            await Task.Delay(25);

            var subscription2 = result.SubscribeAsync(observer2);
            await Task.Delay(50);

            subscription1.Dispose();
            await Task.Delay(150);
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
