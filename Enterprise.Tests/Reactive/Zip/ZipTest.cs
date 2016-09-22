using System;
using System.Linq;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Core.Reactive;
using Enterprise.Core.Reactive.Linq;
using Enterprise.Tests.Reactive.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Enterprise.Core.Reactive.Linq.AsyncObservable;

namespace Enterprise.Tests.Reactive.Zip
{
    [TestClass]
    public sealed class ZipTest
    {
        private const int DefaultTimeout = 1000;

        private const string CategoryReactiveZip = "Reactive.Zip";

        [TestMethod]
        [TestCategory(CategoryReactiveZip)]
        public async Task Simple3()
        {
            var first = AsyncObservable.Range(1, 5);
            var second = AsyncObservable.Repeat(1, 6);

            var query = first.Zip(second, (x, y) => x - y);
            var observer = query.CreateSpyAsyncObserver();

            await query.SubscribeAsync(observer);
            Assert.IsTrue(observer.IsCompleted);
            Assert.IsFalse(observer.Error.InnerExceptions.Any());
            Assert.IsTrue(await observer.Items.SequenceEqualAsync(Enumerable.Range(0, 5)));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveZip)]
        public async Task Simple3E()
        {
            var first = AsyncObservable.Range(1, 5);
            var second = AsyncEnumerable.Repeat(1, 4);

            var query = first.Zip(second, (x, y) => x - y);
            var observer = query.CreateSpyAsyncObserver();

            await query.SubscribeAsync(observer);
            Assert.IsTrue(observer.IsCompleted);
            Assert.IsFalse(observer.Error.InnerExceptions.Any());
            Assert.IsTrue(await observer.Items.SequenceEqualAsync(Enumerable.Range(0, 4)));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveZip)]
        [Timeout(DefaultTimeout)]
        public async Task Infinite3()
        {
            var first = AsyncObservable.Range(1, 5);
            var second = AsyncObservable.Repeat(1);

            var query = first.Zip(second, (x, y) => x - y);
            var observer = query.CreateSpyAsyncObserver();

            await query.SubscribeAsync(observer);
            Assert.IsTrue(observer.IsCompleted);
            Assert.IsFalse(observer.Error.InnerExceptions.Any());
            Assert.IsTrue(await observer.Items.SequenceEqualAsync(Enumerable.Range(0, 5)));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveZip)]
        [Timeout(DefaultTimeout)]
        public async Task Combinations4()
        {
            var first = AsyncObservable.Interval(TimeSpan.FromMilliseconds(100));
            var second = AsyncObservable.Repeat(1, 10);
            var third = AsyncObservable.Repeat(2, 4);

            var query = first.Zip(second, third, (x, y, z) => (int)(x + y) * z);
            var observer = query.CreateSpyAsyncObserver();

            await query.SubscribeAsync(observer);
            Assert.IsTrue(observer.IsCompleted);
            Assert.IsFalse(observer.Error.InnerExceptions.Any());
            Assert.IsTrue(await observer.Items.SequenceEqualAsync(new[] { 2, 4, 6, 8 }));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveZip)]
        [Timeout(DefaultTimeout)]
        public async Task TimeShift3()
        {
            var first = AsyncObservable.Interval(TimeSpan.FromMilliseconds(200)).Take(3);
            var second = "abcdef";

            var query = first.Zip(second, (num, cha) => new { num, cha });
            var observer = query.CreateSpyAsyncObserver();

            await query.SubscribeAsync(observer);
            Assert.IsTrue(observer.IsCompleted);
            Assert.IsFalse(observer.Error.InnerExceptions.Any());

            var expected = new[]
            {
                new { num = 0L, cha = 'a'},
                new { num = 1L, cha = 'b'},
                new { num = 2L, cha = 'c'},
            };

            Assert.IsTrue(await observer.Items.SequenceEqualAsync(expected));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveZip)]
        [Timeout(DefaultTimeout)]
        public async Task Parallel3()
        {
            var first = AsyncObservable.Range(0, 9).SelectMany(
                x => AsyncObservable.Timer(TimeSpan.FromMilliseconds(500)).Select(y => x)).Do(
                (x, ct) => Console.Out.WriteLineAsync("First: " + x));

            var second = AsyncObservable.Range(0, 9).SelectMany(
                x => AsyncObservable.Timer(TimeSpan.FromMilliseconds(500)).Select(y => x)).Do(
                (x, ct) => Console.Out.WriteLineAsync("Second: " + x));

            var query = first.Zip(second, (x, y) => x + y);
            var observer = query.CreateSpyAsyncObserver();

            await query.SubscribeAsync(observer);
            var expected = Enumerable.Range(0, 9).Sum() * 2;

            Assert.IsTrue(observer.IsCompleted);
            Assert.IsFalse(observer.Error.InnerExceptions.Any());
            Assert.AreEqual(9, await observer.Items.CountAsync());
            Assert.AreEqual(expected, await observer.Items.SumAsync());
        }
    }
}
