using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
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
        [Timeout(DefaultTimeout)]
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

            Assert.IsTrue(await source.SequenceEqual(new[] { 1, 2, 3 }));
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
    }
}
