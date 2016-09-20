﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Core.Reactive;
using Enterprise.Core.Reactive.Linq;
using Enterprise.Tests.Reactive.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Enterprise.Core.Reactive.Linq.AsyncObservable;

namespace Enterprise.Tests.Reactive.SelectMany
{
    [TestClass]
    public class SelectManyTest
    {
        private const int DefaultTimeout = 3000;

        private const string CategoryReactiveSelectMany = "Reactive.SelectMany";

        [TestMethod]
        [TestCategory(CategoryReactiveSelectMany)]
        public async Task SimpleOneToMany()
        {
            var source = Return(3);
            var query = source.SelectMany(i => AsyncObservable.Range(1, i));
            var observer = new SpyAsyncObserver<int>();

            await query.SubscribeAsync(observer);
            Assert.IsTrue(await observer.Items.SequenceEqualAsync(Enumerable.Range(1, 3)));
            Assert.IsTrue(observer.IsCompleted);
            Assert.IsFalse(observer.Error.InnerExceptions.Any());
        }

        [TestMethod]
        [TestCategory(CategoryReactiveSelectMany)]
        public async Task SimpleManyToMany()
        {
            var source = AsyncObservable.Range(1, 3);
            var query = source.SelectMany(i => AsyncObservable.Range(1, i));
            var observer = new SpyAsyncObserver<int> { MillisecondsDelay = 0 };

            await query.SubscribeAsync(observer);
            Assert.IsTrue(await observer.Items.SequenceEqualAsync(new[] { 1, 1, 2, 1, 2, 3 }));
            Assert.IsTrue(observer.IsCompleted);
            Assert.IsFalse(observer.Error.InnerExceptions.Any());
        }

        [TestMethod]
        [TestCategory(CategoryReactiveSelectMany)]
        public async Task ReimplementWhereSelect()
        {
            Func<int, char> letter = i => (char)(i + 64);
            var source = AsyncObservable.Range(1, 30);
            var query = source.SelectMany(i =>
            {
                if (0 < i && i < 27)
                {
                    return Return(letter(i));
                }
                else
                {
                    return Empty<char>();
                }
            });

            var observer = new SpyAsyncObserver<char> { MillisecondsDelay = 0 };

            await query.SubscribeAsync(observer);
            Assert.IsTrue(await observer.Items.SequenceEqualAsync("ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.IsTrue(observer.IsCompleted);
            Assert.IsFalse(observer.Error.InnerExceptions.Any());
        }

        [TestMethod]
        [TestCategory(CategoryReactiveSelectMany)]
        [Timeout(DefaultTimeout)]
        public async Task PullVersusPush()
        {
            var enumerableSource = AsyncObservable.Range(1, 3);
            var enumerableResult = enumerableSource.SelectMany(PullSubValues);
            using (var enumerator = enumerableResult.GetAsyncEnumerator())
            {
                while (await enumerator.MoveNextAsync())
                {
                    await Console.Out.WriteLineAsync("MoveNextAsync: " + enumerator.Current);
                }
            }

            Assert.IsTrue(await enumerableResult.SequenceEqual(
                new[] { 10, 11, 12, 20, 21, 22, 30, 31, 32 }));

            var observable = AsyncObservable
                .Interval(TimeSpan.FromMilliseconds(300))
                .Select(i => i + 1)
                .Take(3)
                .SelectMany(PushSubValues);

            var observer = new SpyAsyncObserver<long> { MillisecondsDelay = 0 };
            await observable.SubscribeAsync(observer);

            Assert.IsTrue(await observer.Items.SequenceEqualAsync(
                new long[] { 10, 20, 11, 30, 21, 12, 31, 22, 32 }));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveSelectMany)]
        [Timeout(DefaultTimeout)]
        public async Task QueryExpression()
        {
            var source = AsyncObservable.Range(1, 5);
            var query =
                from i in source
                where i % 2 == 0
                from j in PushSubValues(i)
                select new { i, j };

            await query.ForEachAsync((value, cancellationToken) =>
            {
                return Console.Out.WriteLineAsync("OnNextAsync: " + value);
            }, CancellationToken.None);
        }

        [TestMethod]
        [TestCategory(CategoryReactiveSelectMany)]
        [Timeout(DefaultTimeout)]
        public async Task Tasks()
        {
            var source = AsyncObservable.Range(1, 5);
            var query = source.SelectMany(async (value, cancellationToken) =>
            {
                await Task.Delay(value * 100, cancellationToken);
                return value;
            });

            var observer = new SpyAsyncObserver<int> { MillisecondsDelay = 0 };
            await query.SubscribeAsync(observer);

            Assert.IsTrue(await observer.Items.SequenceEqualAsync(Enumerable.Range(1, 5)));
        }

        private IEnumerable<int> PullSubValues(
            int offset)
        {
            yield return offset * 10;
            yield return (offset * 10) + 1;
            yield return (offset * 10) + 2;
        }

        private IAsyncObservable<long> PushSubValues(
            long offset)
        {
            return AsyncObservable
                .Timer(TimeSpan.Zero, TimeSpan.FromMilliseconds(400))
                .Select(x => (offset * 10) + x)
                .Take(3);
        }
    }
}