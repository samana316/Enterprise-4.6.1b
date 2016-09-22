using System;
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
    public sealed class SelectManyTest
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
            var enumerableSource = AsyncEnumerable.Range(1, 3);
            var enumerableResult = enumerableSource.SelectMany(PullSubValues);
            using (var enumerator = enumerableResult.GetAsyncEnumerator())
            {
                while (await enumerator.MoveNextAsync())
                {
                    await Console.Out.WriteLineAsync("MoveNextAsync: " + enumerator.Current);
                }
            }

            Assert.IsTrue(await enumerableResult.SequenceEqualAsync(
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
        public async Task SelfAsParallel()
        {
            var source = AsyncObservable.Timer(TimeSpan.FromMilliseconds(300)).Repeat(10);
            var query = source.SelectMany(x => Return(x));

            var observer = new SpyAsyncObserver<long> { MillisecondsDelay = 0 };
            await query.SubscribeAsync(observer);
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
        public async Task IntermediateSequenceWithIndex()
        {
            var source = AsyncObservable.Range(1, 5);
            var query = source.SelectMany(
                (x, i) => Return(x).Concat(Return(i)),
                (x, i, y, j) => new { Item = x, Index = i, CollectionItem = y, CollectionIndex = j });

            var observer = query.CreateSpyAsyncObserver();
            observer.MillisecondsDelay = 0;

            await query.SubscribeAsync(observer);

            var expectedItems = new[] { 1, 1, 2, 2, 3, 3, 4, 4, 5, 5 };
            var expectedIndexes = (from x in expectedItems select x - 1).ToArray();
            var expectedCollectionItems = new[] { 1, 0, 2, 1, 3, 2, 4, 3, 5, 4 };
            var expectedCollectionIndexes = await AsyncObservable.Range(0, 2).Repeat(5).ToList();

            var actualItems = observer.Items.Select(x => x.Item).ToArray();
            var actualIndexes = observer.Items.Select(x => x.Index).ToArray();
            var actualCollectionItems = observer.Items.Select(x => x.CollectionItem).ToArray();
            var actualCollectionIndexes = observer.Items.Select(x => x.CollectionIndex).ToArray();

            CollectionAssert.AreEquivalent(expectedItems, actualItems);
            CollectionAssert.AreEquivalent(expectedIndexes, actualIndexes);
            CollectionAssert.AreEquivalent(expectedCollectionItems, actualCollectionItems);
            CollectionAssert.AreEquivalent(expectedCollectionIndexes.ToArray(), actualCollectionIndexes);
        }

        [TestMethod]
        [TestCategory(CategoryReactiveSelectMany)]
        [Timeout(DefaultTimeout)]
        public async Task IntermediateTasksWithIndex()
        {
            var source = AsyncObservable.Range(1, 5);
            var query = source.SelectMany(
               async (x, i, ct) => { await Task.Delay(i, ct); return x + i; },
               (x, i, y) => new { Item = x, Index = i, TaskItem = y });

            var observer = query.CreateSpyAsyncObserver();
            observer.MillisecondsDelay = 0;

            await query.SubscribeAsync(observer);

            var sortedResults =
                from x in observer.Items
                orderby x.Index
                select x;

            var index = 0;
            await sortedResults.ForEachAsync(x => 
            {
                Assert.AreEqual(index, x.Index);
                Assert.AreEqual(index + 1, x.Item);
                Assert.AreEqual(index + index + 1, x.TaskItem);
                index++;
            });
        }

        [TestMethod]
        [TestCategory(CategoryReactiveSelectMany)]
        [Timeout(DefaultTimeout)]
        public async Task Delegates()
        {
            var observer = new SpyAsyncObserver<int> { MillisecondsDelay = 0 };
            var source = AsyncObservable.Range(1, 5);
            var query = source.SelectMany(
               (x, i) => Return(x == 5 ? 1 / (5 - x) : x + i),
                ex => { observer.OnError(ex); return Return(-1); },
                () => Return(0));

            await query.SubscribeAsync(observer);

            Assert.IsTrue(await observer.Items.SequenceEqualAsync(
                new [] { 1, 3, 5, 7, -1, 0 }));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveSelectMany)]
        public async Task Other()
        {
            var source = AsyncObservable.Range(1, 5);
            var query = source.SelectMany(Return(0));
            var observer = new SpyAsyncObserver<int> { MillisecondsDelay = 0 };

            await query.SubscribeAsync(observer);
            Assert.IsTrue(await observer.Items.SequenceEqualAsync(Enumerable.Repeat(0, 5)));
            Assert.IsTrue(observer.IsCompleted);
            Assert.IsFalse(observer.Error.InnerExceptions.Any());
        }

        [TestMethod]
        [TestCategory(CategoryReactiveSelectMany)]
        public async Task Enumerables()
        {
            var source = AsyncObservable.Range(1, 3);
            var query = source.SelectMany(i => AsyncEnumerable.Range(1, i));
            var observer = new SpyAsyncObserver<int> { MillisecondsDelay = 0 };

            await query.SubscribeAsync(observer);
            Assert.IsTrue(await observer.Items.SequenceEqualAsync(new[] { 1, 1, 2, 1, 2, 3 }));
            Assert.IsTrue(observer.IsCompleted);
            Assert.IsFalse(observer.Error.InnerExceptions.Any());
        }

        [TestMethod]
        [TestCategory(CategoryReactiveSelectMany)]
        [Timeout(DefaultTimeout)]
        public async Task Tasks()
        {
            var observer = new SpyAsyncObserver<int> { MillisecondsDelay = 0 };
            var source = AsyncObservable.Range(1, 5);
            var query = source.SelectMany(async (value, cancellationToken) =>
            {
                await Task.Delay(value * 100, cancellationToken);
                return value;
            });

            await query.SubscribeAsync(observer);
            Assert.IsTrue(await observer.Items.SequenceEqualAsync(Enumerable.Range(1, 5)));
            observer.Reset();

            query = source.SelectMany(async (value, index, cancellationToken) =>
            {
                await Task.Delay(value * 100, cancellationToken);
                return value + index;
            });

            await query.SubscribeAsync(observer);
            Assert.IsTrue(await observer.Items.SequenceEqualAsync(new[] { 1, 3, 5, 7, 9 }));
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
