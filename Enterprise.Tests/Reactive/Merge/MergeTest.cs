﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
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
        private const int DefaultTimeout = 3000;

        private const string CategoryReactiveMerge = "Reactive.Merge";

        [TestMethod]
        [TestCategory(CategoryReactiveMerge)]
        public async Task Simple()
        {
            var s1 = AsyncObservable.Range(1, 5);
            var s2 = AsyncObservable.Range(101, 5);

            var query = s1.Merge(s2);
            var observer = new SpyAsyncObserver<int>();
            await query.SubscribeAsync(observer);

            // Can't compare the entire sequence due to race condition.
            Assert.AreEqual(10, await observer.Items.CountAsync());
        }

        [TestMethod]
        [TestCategory(CategoryReactiveMerge)]
        [Timeout(DefaultTimeout)]
        public async Task Infinite()
        {
            var s1 = AsyncObservable.Repeat(1);
            var s2 = AsyncObservable.Repeat(100);

            var query = s1.Merge(s2).Take(5);
            var observer = new SpyAsyncObserver<int>();
            await query.SubscribeAsync(observer);

            // Can't compare the entire sequence due to race condition.
            Assert.AreEqual(5, await observer.Items.CountAsync());
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

        [TestMethod]
        [TestCategory(CategoryReactiveMerge)]
        public async Task WithMaxConcurrent()
        {
            var dueTimes = new long[] { 10, 100, 200, 10, 100 };
            var sources = Create<IAsyncObservable<long>>(async (yield, cancellationToken) =>
            {
                foreach (var dueTime in dueTimes)
                {
                    var timer = AsyncObservable
                        .Timer(TimeSpan.FromMilliseconds(dueTime))
                        .Select(i => i + dueTime);

                    await yield.ReturnAsync(timer, cancellationToken);
                }
            });
            
            var query = sources.Merge(3);
            var observer = new SpyAsyncObserver<long>();

            var stopwatch = Stopwatch.StartNew();
            await query.SubscribeAsync(observer);
            stopwatch.Stop();

            Assert.IsTrue(await observer.Items.SequenceEqualAsync(dueTimes));
            Assert.IsTrue(stopwatch.ElapsedMilliseconds < dueTimes.Sum());
        }

        [TestMethod]
        [TestCategory(CategoryReactiveMerge)]
        public async Task WithException()
        {
            Exception exception = new NotImplementedException();

            var x = 0;
            try
            {
                var y = 1 / x;
            }
            catch (DivideByZeroException dbze)
            {
                exception = dbze;
            }

            var dueTimes = new long[] { 10, 100, 200, 10, 100 };
            var sources = Create<IAsyncObservable<long>>(async (yield, cancellationToken) =>
            {
                var @throw = Throw<long>(exception);
                await yield.ReturnAsync(@throw, cancellationToken);

                foreach (var dueTime in dueTimes)
                {
                    var timer = AsyncObservable
                        .Timer(TimeSpan.FromMilliseconds(dueTime))
                        .Select(i => i + dueTime);

                    await yield.ReturnAsync(timer, cancellationToken);
                }
            });

            var query = sources.Merge(3);
            var observer = new SpyAsyncObserver<long>();
            
            await query.SubscribeAsync(observer);
        }

        [TestMethod]
        [TestCategory(CategoryReactiveMerge)]
        [Timeout(DefaultTimeout)]
        public async Task MarbleDiagram()
        {
            var sources = LiveSearch.CreateMock<string>(TimeSpan.FromMilliseconds(100));
            var query = sources.Merge().Select(x => Convert.ToInt32(x));

            var observer = query.CreateSpyAsyncObserver();
            await query.SubscribeAsync(observer, CancellationToken.None);

            Assert.IsTrue(observer.IsCompleted);
            Assert.IsFalse(observer.Error.InnerExceptions.Any());
            Assert.AreEqual(10, await observer.Items.CountAsync());
            Assert.IsTrue(await observer.Items.SequenceEqualAsync(new[] { 1,1,2,1,2,3,1,2,3,2 }));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveMerge)]
        [Timeout(DefaultTimeout)]
        public async Task TasksWithException()
        {
            var sources = Create<Task<int>>((yield, cancellationToken) =>
            {
                var tasks =
                    from dueTime in new int[] { 300, 200, 100 }
                    select Task.Delay(dueTime, cancellationToken).ContinueWith(async t =>
                    {
                        if (dueTime == 300)
                        {
                            var x = 1 / (300 - dueTime);
                        }

                        await t;

                        return dueTime;
                    }).Unwrap();

                return yield.ReturnAllAsync(tasks, cancellationToken);
            });

            var query = sources.Merge();
            var observer = new SpyAsyncObserver<int>();
            await query.SubscribeAsync(observer);

            var expected = new[] { 100, 200 };
            Assert.IsTrue(observer.IsCompleted);
            Assert.IsTrue(observer.Error.InnerExceptions.Any());
            Assert.IsTrue(await observer.Items.SequenceEqualAsync(expected));
        }
    }
}
