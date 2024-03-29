﻿using System;
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
        private const int DefaultTimeout = 2000;

        private const string CategoryReactiveZip = "Reactive.Zip";

        [TestMethod]
        [TestCategory(CategoryReactiveZip)]
        [Timeout(DefaultTimeout)]
        public async Task Empty1()
        {
            var sources = AsyncEnumerable.Empty<IAsyncObservable<int>>();
            var query = sources.Zip();

            var observer = query.CreateSpyAsyncObserver();
            await query.SubscribeAsync(observer);

            Assert.IsTrue(observer.IsCompleted);
            Assert.IsFalse(observer.Error.InnerExceptions.Any());
            Assert.IsFalse(await observer.Items.AnyAsync());
        }

        [TestMethod]
        [TestCategory(CategoryReactiveZip)]
        [Timeout(DefaultTimeout)]
        public async Task Throw1()
        {
            var sources = new[]
            {
                Throw<long>(new InvalidOperationException())
            };

            var query = sources.Zip();
            var observer = query.CreateSpyAsyncObserver();

            await query.SubscribeAsync(observer);
            Assert.IsTrue(observer.IsCompleted);
            Assert.IsTrue(observer.Error.InnerExceptions.Any());
            Assert.IsFalse(await observer.Items.AnyAsync());
        }

        [TestMethod]
        [TestCategory(CategoryReactiveZip)]
        [Timeout(DefaultTimeout)]
        public async Task Enumerable1()
        {
            var sources = new[]
            {
                AsyncObservable.Range(0, 3),
                AsyncObservable.Range(10, 3),
                AsyncObservable.Range(20, 3),
            };

            var query = sources.Zip();

            var observer = query.CreateSpyAsyncObserver();
            await query.SubscribeAsync(observer);

            Assert.IsTrue(observer.IsCompleted);
            Assert.IsFalse(observer.Error.InnerExceptions.Any());

            var results = await observer.Items.Select(
                x => x.ToArray()).ToListAsync();

            Assert.AreEqual(3, results.Count);
            CollectionAssert.AreEqual(new[] { 0, 10, 20 }, results[0]);
            CollectionAssert.AreEqual(new[] { 1, 11, 21 }, results[1]);
            CollectionAssert.AreEqual(new[] { 2, 12, 22 }, results[2]);
        }

        [TestMethod]
        [TestCategory(CategoryReactiveZip)]
        [Timeout(DefaultTimeout)]
        public async Task Infinite1()
        {
            var sources = new[]
            {
                AsyncObservable.Repeat(1),
                AsyncObservable.Repeat(2),
                AsyncObservable.Repeat(3),
            };

            var query = sources.Zip().Take(3);

            var observer = query.CreateSpyAsyncObserver();
            observer.MillisecondsDelay = 0;
            await query.SubscribeAsync(observer);

            Assert.IsTrue(observer.IsCompleted);
            Assert.IsFalse(observer.Error.InnerExceptions.Any());

            var results = await observer.Items.Select(
                x => x.ToArray()).ToListAsync();

            Assert.AreEqual(3, results.Count);
            foreach (var result in results)
            {
                CollectionAssert.AreEqual(new[] { 1, 2, 3 }, result);
            }
        }

        [TestMethod]
        [TestCategory(CategoryReactiveZip)]
        [Timeout(DefaultTimeout)]
        public async Task EnumerableTimeShift1()
        {
            var sources = new[]
            {
                AsyncObservable.Interval(TimeSpan.FromMilliseconds(250)),
                AsyncObservable.Interval(TimeSpan.FromMilliseconds(100)).Select(x => x + 10),
                AsyncObservable.Interval(TimeSpan.FromMilliseconds(50)).Select(x => x + 20),
            };

            var query = sources.Zip().Take(3);

            var observer = query.CreateSpyAsyncObserver();
            await query.SubscribeAsync(observer);

            Assert.IsTrue(observer.IsCompleted);
            Assert.IsFalse(observer.Error.InnerExceptions.Any());

            var results = await observer.Items.Select(
                x => x.Select(Convert.ToInt32).ToArray()).ToListAsync();

            Assert.AreEqual(3, results.Count);
            CollectionAssert.AreEqual(new[] { 0, 10, 20 }, results[0]);
            CollectionAssert.AreEqual(new[] { 1, 11, 21 }, results[1]);
            CollectionAssert.AreEqual(new[] { 2, 12, 22 }, results[2]);
        }

        [TestMethod]
        [TestCategory(CategoryReactiveZip)]
        [Timeout(DefaultTimeout)]
        public async Task Empty3()
        {
            var first = AsyncObservable.Range(1, 5);
            var second = Empty<int>();

            var query = first.Zip(second, (x, y) => x - y);
            var observer = query.CreateSpyAsyncObserver();

            await query.SubscribeAsync(observer);
            Assert.IsTrue(observer.IsCompleted);
            Assert.IsFalse(observer.Error.InnerExceptions.Any());
            Assert.IsFalse(await observer.Items.AnyAsync());
        }

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
        public async Task Throw3()
        {
            var first = AsyncObservable.Range(1, 5);
            var second = Throw<int>(new InvalidOperationException());

            var query = first.Zip(second, (x, y) => x - y);
            var observer = query.CreateSpyAsyncObserver();

            await query.SubscribeAsync(observer);
            Assert.IsTrue(observer.IsCompleted);
            Assert.IsTrue(observer.Error.InnerExceptions.Any());
            Assert.IsFalse(await observer.Items.AnyAsync());
        }

        [TestMethod]
        [TestCategory(CategoryReactiveZip)]
        [Timeout(DefaultTimeout)]
        public async Task Infinite3()
        {
            var doer = new SpyAsyncObserver<int> { MillisecondsDelay = 100 };
            var first = AsyncObservable.Repeat(1);
            var second = AsyncObservable.Repeat(2);

            var query = first.Zip(second, (x, y) => x + y).Take(5);
            var observer = query.CreateSpyAsyncObserver();
            observer.MillisecondsDelay = 0;

            await query.SubscribeAsync(observer);
            Assert.IsTrue(observer.IsCompleted);
            Assert.IsFalse(observer.Error.InnerExceptions.Any());
            Assert.IsTrue(await observer.Items.SequenceEqualAsync(Enumerable.Repeat(3, 5)));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveZip)]
        [Timeout(DefaultTimeout)]
        public async Task TimeShiftInfinite3()
        {
            var delay = TimeSpan.FromMilliseconds(100);
            var first = AsyncObservable.Interval(delay);
            var second = AsyncObservable.Interval(delay).Select(x => x + 100);

            var query = first.Zip(second, (x, y) => y - x).Take(5).Select(Convert.ToInt32);
            var observer = query.CreateSpyAsyncObserver();
            observer.MillisecondsDelay = 0;

            await query.SubscribeAsync(observer);
            Assert.IsTrue(observer.IsCompleted);
            Assert.IsFalse(observer.Error.InnerExceptions.Any());
            Assert.IsTrue(await observer.Items.SequenceEqualAsync(Enumerable.Repeat(100, 5)));
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
            var first = AsyncObservable
                .Interval(TimeSpan.FromMilliseconds(200))
                .Select(Convert.ToInt32);

            var second = "abcdef".ToAsyncObservable();

            var query = first.Zip(second, (num, cha) => new { num, cha }).Take(3);
            var observer = query.CreateSpyAsyncObserver();

            await query.SubscribeAsync(observer);
            Assert.IsTrue(observer.IsCompleted);
            Assert.IsFalse(observer.Error.InnerExceptions.Any());

            var expected = new[]
            {
                new { num = 0, cha = 'a'},
                new { num = 1, cha = 'b'},
                new { num = 2, cha = 'c'},
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

        [TestMethod]
        [TestCategory(CategoryReactiveZip)]
        [Timeout(DefaultTimeout)]
        public async Task Combinations16()
        {
            var sources = await Repeat<IAsyncObservable<int>>(Return(1), 16).ToList();

            var query = sources[0].Zip(
                sources[1],
                sources[2],
                sources[3],
                sources[4],
                sources[5],
                sources[6],
                sources[7],
                sources[8],
                sources[9],
                sources[10],
                sources[11],
                sources[12],
                sources[13],
                sources[14],
                sources[15],
                this.DummySelector);

            var observer = query.CreateSpyAsyncObserver();
            await query.SubscribeAsync(observer);

            Assert.IsTrue(observer.IsCompleted);
            Assert.IsFalse(observer.Error.InnerExceptions.Any());
            Assert.AreEqual(1, await observer.Items.CountAsync());
            Assert.AreEqual(42, await observer.Items.SingleOrDefaultAsync());
        }

        private int DummySelector(
            int arg1, int arg2, int arg3, int arg4, 
            int arg5, int arg6, int arg7, int arg8, 
            int arg9, int arg10, int arg11, int arg12, 
            int arg13, int arg14, int arg15, int arg16)
        {
            return 42;
        }
    }
}
