using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Core.Reactive.Linq;
using Enterprise.Tests.Reactive.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Enterprise.Core.Reactive.Linq.AsyncObservable;

namespace Enterprise.Tests.Reactive.Buffer
{
    [TestClass]
    public sealed class BufferTest
    {
        private const int DefaultTimeout = 3000;

        private const string CategoryReactiveBuffer = "Reactive.Buffer";

        [TestMethod]
        [TestCategory(CategoryReactiveBuffer)]
        public async Task Simple()
        {
            var source = AsyncObservable.Range(1, 20);
            var query = source.Buffer(4);

            var observer = query.CreateSpyAsyncObserver();
            await query.SubscribeAsync(observer);

            Assert.IsTrue(observer.IsCompleted);
            Assert.IsFalse(observer.Error.InnerExceptions.Any());
            Assert.AreEqual(5, await observer.Items.CountAsync());

            var buffers = await observer.Items.ToListAsync();
            Assert.IsTrue(buffers[0].SequenceEqual(Enumerable.Range(1, 4)));
            Assert.IsTrue(buffers[1].SequenceEqual(Enumerable.Range(5, 4)));
            Assert.IsTrue(buffers[2].SequenceEqual(Enumerable.Range(9, 4)));
            Assert.IsTrue(buffers[3].SequenceEqual(Enumerable.Range(13, 4)));
            Assert.IsTrue(buffers[4].SequenceEqual(Enumerable.Range(17, 4)));
        }

        //[TestMethod]
        //[TestCategory(CategoryReactiveBuffer)]
        //[Timeout(DefaultTimeout)]
        //public async Task SimpleByTime()
        //{
        //    var idealBatchSize = 15;
        //    var maxTimeDelay = TimeSpan.FromSeconds(3);
        //    var source = AsyncObservable.Interval(TimeSpan.FromSeconds(1)).Take(10)
        //        .Concat(AsyncObservable.Interval(TimeSpan.FromSeconds(0.01)).Take(100));

        //    var query = source.Buffer(maxTimeDelay, idealBatchSize);

        //    var observer = query.CreateSpyAsyncObserver();
        //    await query.SubscribeAsync(observer);

        //}

        [TestMethod]
        [TestCategory(CategoryReactiveBuffer)]
        [Timeout(DefaultTimeout)]
        public async Task ByCountOverlap()
        {
            var source = AsyncObservable.Range(0, 4);
            var query = source.Buffer(3, 1);

            var observer = query.CreateSpyAsyncObserver();
            await query.SubscribeAsync(observer);

            Assert.IsTrue(observer.IsCompleted);
            Assert.IsFalse(observer.Error.InnerExceptions.Any());
            Assert.AreEqual(4, await observer.Items.CountAsync());

            var buffers = await observer.Items.ToListAsync();
            Assert.IsTrue(buffers[0].SequenceEqual(new[] { 0, 1, 2 }));
            Assert.IsTrue(buffers[1].SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(buffers[2].SequenceEqual(new[] { 2, 3 }));
            Assert.IsTrue(buffers[3].SequenceEqual(new[] { 3 }));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveBuffer)]
        [Timeout(DefaultTimeout)]
        public async Task ByCountStandard()
        {
            var source = AsyncObservable.Range(0, 7);
            var query = source.Buffer(3, 3);

            var observer = query.CreateSpyAsyncObserver();
            await query.SubscribeAsync(observer);

            Assert.IsTrue(observer.IsCompleted);
            Assert.IsFalse(observer.Error.InnerExceptions.Any());
            Assert.AreEqual(3, await observer.Items.CountAsync());

            var buffers = await observer.Items.ToListAsync();
            Assert.IsTrue(buffers[0].SequenceEqual(new[] { 0, 1, 2 }));
            Assert.IsTrue(buffers[1].SequenceEqual(new[] { 3, 4, 5 }));
            Assert.IsTrue(buffers[2].SequenceEqual(new[] { 6 }));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveBuffer)]
        [Timeout(DefaultTimeout)]
        public async Task ByCountSkip()
        {
            var source = AsyncObservable.Range(0, 10);
            var query = source.Buffer(3, 5);

            var observer = query.CreateSpyAsyncObserver();
            await query.SubscribeAsync(observer);

            Assert.IsTrue(observer.IsCompleted);
            Assert.IsFalse(observer.Error.InnerExceptions.Any());
            Assert.AreEqual(2, await observer.Items.CountAsync());

            var buffers = await observer.Items.ToListAsync();
            Assert.IsTrue(buffers[0].SequenceEqual(new[] { 0, 1, 2 }));
            Assert.IsTrue(buffers[1].SequenceEqual(new[] { 5, 6, 7 }));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveBuffer)]
        [Timeout(DefaultTimeout)]
        public async Task ByTimeStandard()
        {
            var source = AsyncObservable.Interval(TimeSpan.FromMilliseconds(100)).Take(7);
            var query = source.Buffer(TimeSpan.FromMilliseconds(300), TimeSpan.FromMilliseconds(300));

            var observer = query.CreateSpyAsyncObserver();
            await query.SubscribeAsync(observer);

            Assert.IsTrue(observer.IsCompleted);
            Assert.IsFalse(observer.Error.InnerExceptions.Any());
            Assert.AreEqual(3, await observer.Items.CountAsync());

            var buffers = await observer.Items.ToListAsync();
            Assert.IsTrue(buffers[0].SequenceEqual(new long[] { 0, 1, 2 }));
            Assert.IsTrue(buffers[1].SequenceEqual(new long[] { 3, 4, 5 }));
            Assert.IsTrue(buffers[2].SequenceEqual(new long[] { 6 }));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveBuffer)]
        [Timeout(DefaultTimeout)]
        public async Task ByTimeOverlap()
        {
            var source = AsyncObservable.Interval(TimeSpan.FromMilliseconds(100)).Take(4);
            var query = source.Buffer(TimeSpan.FromMilliseconds(300), TimeSpan.FromMilliseconds(100));

            var observer = query.CreateSpyAsyncObserver();
            await query.SubscribeAsync(observer);

            Assert.IsTrue(observer.IsCompleted);
            Assert.IsFalse(observer.Error.InnerExceptions.Any());
            Assert.AreEqual(4, await observer.Items.CountAsync());

            var buffers = await observer.Items.ToListAsync();
            Assert.IsTrue(buffers[0].SequenceEqual(new long[] { 0, 1, 2 }));
            Assert.IsTrue(buffers[1].SequenceEqual(new long[] { 1, 2, 3 }));
            Assert.IsTrue(buffers[2].SequenceEqual(new long[] { 2, 3 }));
            Assert.IsTrue(buffers[3].SequenceEqual(new long[] { 3 }));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveBuffer)]
        [Timeout(DefaultTimeout)]
        public async Task ByTimeSkip()
        {
            var source = AsyncObservable.Interval(TimeSpan.FromMilliseconds(100)).Take(10);
            var query = source.Buffer(TimeSpan.FromMilliseconds(300), TimeSpan.FromMilliseconds(500));

            var observer = query.CreateSpyAsyncObserver();
            await query.SubscribeAsync(observer);

            Assert.IsTrue(observer.IsCompleted);
            Assert.IsFalse(observer.Error.InnerExceptions.Any());
            Assert.AreEqual(2, await observer.Items.CountAsync());

            var buffers = await observer.Items.ToListAsync();
            Assert.IsTrue(buffers[0].SequenceEqual(new long[] { 0, 1, 2 }));
            Assert.IsTrue(buffers[1].SequenceEqual(new long[] { 5, 6, 7 }));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveBuffer)]
        [Timeout(DefaultTimeout)]
        public async Task ByCountStandardInfinite()
        {
            var source = AsyncObservable.Range(1, 20).Repeat();
            var query = source.Buffer(4).Take(5);

            var observer = query.CreateSpyAsyncObserver();
            await query.SubscribeAsync(observer);

            Assert.IsTrue(observer.IsCompleted);
            Assert.IsFalse(observer.Error.InnerExceptions.Any());
            Assert.AreEqual(5, await observer.Items.CountAsync());

            var buffers = await observer.Items.ToListAsync();
            Assert.IsTrue(buffers[0].SequenceEqual(Enumerable.Range(1, 4)));
            Assert.IsTrue(buffers[1].SequenceEqual(Enumerable.Range(5, 4)));
            Assert.IsTrue(buffers[2].SequenceEqual(Enumerable.Range(9, 4)));
            Assert.IsTrue(buffers[3].SequenceEqual(Enumerable.Range(13, 4)));
            Assert.IsTrue(buffers[4].SequenceEqual(Enumerable.Range(17, 4)));
        }
    }
}
