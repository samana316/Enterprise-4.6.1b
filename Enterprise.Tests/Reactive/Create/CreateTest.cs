﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Common.Runtime.CompilerServices;
using Enterprise.Core.Linq;
using Enterprise.Core.Reactive;
using Enterprise.Core.Reactive.Linq;
using Enterprise.Tests.Reactive.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Enterprise.Core.Reactive.Linq.AsyncObservable;

namespace Enterprise.Tests.Reactive.Create
{
    [TestClass]
    public sealed class CreateTest
    {
        private const int DefaultTimeout = 1000;

        private const string CategoryReactiveCreate = "Reactive.Create";

        [TestMethod]
        [TestCategory(CategoryReactiveCreate)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullBuilderThrowsNullArgumentException()
        {
            var source = Create<int>(null);
        }

        [TestMethod]
        [TestCategory(CategoryReactiveCreate)]
        public async Task SingleTask()
        {
            var source = Create<int>((yield, cancellationToken) =>
            {
                return yield.ReturnAsync(1, cancellationToken);
            });

            Assert.IsTrue(await source.SequenceEqual(new[] { 1 }));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveCreate)]
        [Timeout(DefaultTimeout)]
        public async Task ChainedTasks()
        {
            var source = Create<int>((yield, cancellationToken) =>
            {
                return yield.ReturnAsync(1, cancellationToken)
                .ContinueWith(t => yield.ReturnAsync(2, cancellationToken)).Unwrap()
                .ContinueWith(t => yield.ReturnAsync(3, cancellationToken)).Unwrap();
            });

            Assert.IsTrue(await source.SequenceEqual(new[] { 1, 2, 3 }));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveCreate)]
        [Timeout(DefaultTimeout)]
        public async Task MultipleTasksAsync()
        {
            var source = Create<int>(async (yield, cancellationToken) =>
            {
                var task1 = yield.ReturnAsync(1, cancellationToken);
                var task2 = yield.ReturnAsync(2, cancellationToken);

                await task1;
                await task2;
            });

            Assert.IsTrue(await source.SequenceEqual(new[] { 1, 2 }));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveCreate)]
        public async Task SimpleAsync()
        {
            var source = Create<int>(async (yield, cancellationToken) =>
            {
                await yield.ReturnAsync(1, cancellationToken);
                await yield.ReturnAsync(2, cancellationToken);
                await yield.ReturnAsync(3, cancellationToken);
            });

            Assert.IsTrue(await source.SequenceEqual(new[] { 1, 2, 3 }));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveCreate)]
        public async Task SimpleLoopAsync()
        {
            var source = Create<int>(async (yield, cancellationToken) =>
            {
                for (var i = 1; i <= 5; i++)
                {
                    if (i % 2 != 0)
                    {
                        await yield.ReturnAsync(i, cancellationToken);
                    }
                }
            });

            Assert.IsTrue(await source.SequenceEqual(new[] { 1, 3, 5 }));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveCreate)]
        [ExpectedException(typeof(NotImplementedException))]
        public async Task SimpleThrow()
        {
            var source = Create<int>((yield, cancellationToken) =>
            {
                throw new NotImplementedException();
            });

            await source;
        }

        [TestMethod]
        [TestCategory(CategoryReactiveCreate)]
        [ExpectedException(typeof(NotImplementedException))]
        public async Task SimpleThrowAsync()
        {
            var source = Create<int>(async (yield, cancellationToken) =>
            {
                await Task.Yield();

                throw new NotImplementedException();
            });

            await source;
        }

        [TestMethod]
        [TestCategory(CategoryReactiveCreate)]
        [ExpectedException(typeof(NotImplementedException))]
        public async Task SimpleThrowTask()
        {
            var source = Create<int>((yield, cancellationToken) =>
            {
                var taskCompletionSource = new TaskCompletionSource<bool>();
                taskCompletionSource.SetException(new NotImplementedException());

                return taskCompletionSource.Task;
            });

            await source;
        }

        [TestMethod]
        [TestCategory(CategoryReactiveCreate)]
        //[Timeout(DefaultTimeout)]
        public async Task InfiniteLoopWithBreak()
        {
            var source = Create<int>(async (yield, cancellationToken) =>
            {
                var i = 0;
                while (true)
                {
                    i++;
                    if (i > 3)
                    {
                        yield.Break();
                    }
                    await yield.ReturnAsync(i, cancellationToken);
                }
            });

            var observer = source.CreateSpyAsyncObserver();
            await source.SubscribeAsync(observer);

            Assert.IsTrue(await observer.Items.SequenceEqualAsync(new[] { 1, 2, 3 }));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveCreate)]
        [Timeout(DefaultTimeout)]
        [ExpectedException(typeof(DivideByZeroException))]
        public async Task InfiniteLoopWithThrow()
        {
            var source = Create<int>(async (yield, cancellationToken) =>
            {
                var i = 10;
                while (true)
                {
                    i--;
                    var x = i + 10 / i;

                    await yield.ReturnAsync(x, cancellationToken);
                }
            });

            await source;
        }

        [TestMethod]
        [TestCategory(CategoryReactiveCreate)]
        [Timeout(DefaultTimeout)]
        public async Task InfiniteLoopWithTimeout()
        {
            var source = Create<long>(async (yield, cancellationToken) =>
            {
                var i = 0;
                while (true)
                {
                    i++;
                    await yield.ReturnAsync(i, cancellationToken);
                    await Task.Delay(10);
                }
            });

            var observer = new SpyAsyncObserver<long>();
            try
            {
                using (var cancellationTokenSource = new CancellationTokenSource(100))
                {
                    await source.SubscribeAsync(observer, cancellationTokenSource.Token);
                }
            }
            catch (OperationCanceledException exception)
            {
                Trace.WriteLine(await observer.Items.CountAsync());
                Trace.WriteLine(exception, "OnError");
            }

            Assert.IsTrue(await observer.Items.CountAsync() > 0);
        }

        [TestMethod]
        [TestCategory(CategoryReactiveCreate)]
        [Timeout(DefaultTimeout)]
        public async Task ParallelTasks()
        {
            var source = Create<int>((yield, cancellationToken) =>
            {
                var tasks = new List<Task>();
                for (var i = 1; i <= 3; i++)
                {
                    tasks.Add(yield.ReturnAsync(i, cancellationToken));
                }

                return Task.WhenAll(tasks);
            });

            Assert.IsTrue(await source.SequenceEqual(new[] { 1, 2, 3 }));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveCreate)]
        [Timeout(DefaultTimeout)]
        public async Task Nested()
        {
            var source = Create<IAsyncObservable<int>>(async (yield, cancellationToken) =>
            {
                var arrays = new[]
                {
                    new[] {1,2,3},
                    new[] {4,5,6},
                    new[] {7,8,9},
                };

                foreach (var array in arrays)
                {
                    await Task.Delay(1, cancellationToken);
                    await yield.ReturnAsync(array.ToAsyncObservable(), cancellationToken);
                }
            });

            var results = new List<int>();

            await source.ForEachAsync((child, ct) => 
            {
                return child.ForEachAsync(async (item, ct2) => 
                {
                    await Task.Yield();

                    results.Add(item);
                }, CancellationToken.None);
            }, CancellationToken.None);

            Assert.IsTrue(await AsyncEnumerable.Range(1, 9).SequenceEqualAsync(results));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveCreate)]
        [Timeout(DefaultTimeout)]
        public async Task ParallelSubscriptions()
        {
            var source1 = Create<int>(async (yield, cancellationToken) =>
            {
                await yield.ReturnAsync(1, cancellationToken);
                await yield.ReturnAsync(2, cancellationToken);
                await yield.ReturnAsync(3, cancellationToken);
                yield.Break();
            }).Take(3);

            var source2 = Create<int>(async (yield, cancellationToken) =>
            {
                await yield.ReturnAsync(1, cancellationToken);
                await yield.ReturnAsync(2, cancellationToken);
                await yield.ReturnAsync(3, cancellationToken);
                await yield.ReturnAsync(4, cancellationToken);
                await yield.ReturnAsync(5, cancellationToken);
                yield.Break();
            }).Take(5).Select(x => x);

            var observer = new SpyAsyncObserver<int>();

            await Task.WhenAll(
                source1.ForEachAsync(observer.OnNextAsync, CancellationToken.None),
                source2.ForEachAsync(observer.OnNextAsync, CancellationToken.None));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveCreate)]
        [Timeout(DefaultTimeout)]
        public async Task InfiniteLoopWithBreakParallel()
        {
            var source1 = Create<int>(async (yield, cancellationToken) =>
            {
                var i = 0;
                while (true)
                {
                    i++;
                    await Task.Delay(10, cancellationToken);
                    await yield.ReturnAsync(i, cancellationToken);
                }
            });

            var source2 = Create<int>(async (yield, cancellationToken) =>
            {
                var i = 100;
                while (true)
                {
                    i++;
                    await Task.Delay(10, cancellationToken);
                    await yield.ReturnAsync(i, cancellationToken);
                }
            });

            var query1 = Create<int>((yield, cancellationToken) =>
            {
                var task1 = yield.ReturnAllAsync(source1, cancellationToken);
                var task2 = yield.ReturnAllAsync(source2, cancellationToken);

                return Task.WhenAll(task1, task2);
            });

            var query2 = query1.Take(5);
            var observer = query2.CreateSpyAsyncObserver();
            await query2.SubscribeAsync(observer);

            Assert.IsTrue(observer.IsCompleted);
            Assert.IsFalse(observer.Error.InnerExceptions.Any());
            Assert.AreEqual(5, await observer.Items.CountAsync());
        }
    }
}
