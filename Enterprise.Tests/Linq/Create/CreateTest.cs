using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Enterprise.Core.Linq.AsyncEnumerable;

namespace Enterprise.Tests.Linq.Create
{
    [TestClass]
    public sealed class CreateTest
    {
        private const int DefaultTimeout = 1000;
        private const string CategoryLinqCreate = "Linq.Create";

        [TestMethod]
        [TestCategory(CategoryLinqCreate)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullBuilderThrowsNullArgumentException()
        {
            var source = Create<int>(null);
        }

        [TestMethod]
        [TestCategory(CategoryLinqCreate)]
        public async Task SingleTask()
        {
            var source = Create<int>((yield, cancellationToken) =>
            {
                return yield.ReturnAsync(1, cancellationToken);
            });

            Assert.IsTrue(await source.SequenceEqualAsync(new[] { 1 }));
        }

        [TestMethod]
        [TestCategory(CategoryLinqCreate)]
        [Timeout(DefaultTimeout)]
        public async Task ChainedTasks()
        {
            var source = Create<int>((yield, cancellationToken) =>
            {
                return yield.ReturnAsync(1, cancellationToken)
                .ContinueWith(t => yield.ReturnAsync(2, cancellationToken)).Unwrap()
                .ContinueWith(t => yield.ReturnAsync(3, cancellationToken)).Unwrap();
            });

            Assert.IsTrue(await source.SequenceEqualAsync(new[] { 1, 2, 3 }));
        }

        //[TestMethod]
        [TestCategory(CategoryLinqCreate)]
        [Timeout(DefaultTimeout)]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task MultipleTasksAsync()
        {
            var source = Create<int>(async (yield, cancellationToken) =>
            {
                var task1 = yield.ReturnAsync(1, cancellationToken);
                var task2 = yield.ReturnAsync(2, cancellationToken);

                await task1;
                await task2;
            });

            Assert.IsTrue(await source.SequenceEqualAsync(new[] { 1, 2 }));
        }

        [TestMethod]
        [TestCategory(CategoryLinqCreate)]
        public async Task SimpleAsync()
        {
            var source = Create<int>(async (yield, cancellationToken) => 
            {
                await yield.ReturnAsync(1, cancellationToken);
                await yield.ReturnAsync(2, cancellationToken);
                await yield.ReturnAsync(3, cancellationToken);
            });

            Assert.IsTrue(await source.SequenceEqualAsync(new[] { 1, 2, 3 }));
        }

        [TestMethod]
        [TestCategory(CategoryLinqCreate)]
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

            Assert.IsTrue(await source.SequenceEqualAsync(new[] { 1, 3, 5 }));
        }

        [TestMethod]
        [TestCategory(CategoryLinqCreate)]
        [ExpectedException(typeof(NotImplementedException))]
        public async Task SimpleThrow()
        {
            var source = Create<int>((yield, cancellationToken) =>
            {
                throw new NotImplementedException();
            });

            using (var enumerator = source.GetAsyncEnumerator())
            {
                while (await enumerator.MoveNextAsync())
                {
                    Trace.WriteLine(enumerator.Current);
                }
            }
        }

        [TestMethod]
        [TestCategory(CategoryLinqCreate)]
        [ExpectedException(typeof(NotImplementedException))]
        public async Task SimpleThrowAsync()
        {
            var source = Create<int>(async (yield, cancellationToken) =>
            {
                await Task.Yield();

                throw new NotImplementedException();
            });

            using (var enumerator = source.GetAsyncEnumerator())
            {
                while (await enumerator.MoveNextAsync())
                {
                    Trace.WriteLine(enumerator.Current);
                }
            }
        }

        [TestMethod]
        [TestCategory(CategoryLinqCreate)]
        [ExpectedException(typeof(NotImplementedException))]
        public async Task SimpleThrowTask()
        {
            var source = Create<int>((yield, cancellationToken) =>
            {
                var taskCompletionSource = new TaskCompletionSource<bool>();
                taskCompletionSource.SetException(new NotImplementedException());

                return taskCompletionSource.Task;
            });

            using (var enumerator = source.GetAsyncEnumerator())
            {
                while (await enumerator.MoveNextAsync())
                {
                    Trace.WriteLine(enumerator.Current);
                }
            }
        }

        [TestMethod]
        [TestCategory(CategoryLinqCreate)]
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
            
            Assert.IsTrue(await source.SequenceEqualAsync(new[] { 1, 2, 3 }));
        }

        [TestMethod]
        [TestCategory(CategoryLinqCreate)]
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

            using (var enumerator = source.GetAsyncEnumerator())
            {
                while (await enumerator.MoveNextAsync())
                {
                    Trace.WriteLine(enumerator.Current);
                }
            }
        }

        [TestMethod]
        [TestCategory(CategoryLinqCreate)]
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
                }
            });

            var count = 0;
            try
            {
                using (var cancellationTokenSource = new CancellationTokenSource(DefaultTimeout / 2))
                {
                    using (var enumerator = source.GetAsyncEnumerator())
                    {
                        while (await enumerator.MoveNextAsync(cancellationTokenSource.Token))
                        {
                            count++;
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Trace.WriteLine(count);
            }

            Assert.IsTrue(count > 0);
        }

        //[TestMethod]
        [TestCategory(CategoryLinqCreate)]
        [Timeout(DefaultTimeout)]
        [ExpectedException(typeof(OperationCanceledException))]
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

            using (var cancellationTokenSource = new CancellationTokenSource(DefaultTimeout / 2))
            {
                using (var enumerator = source.GetAsyncEnumerator())
                {
                    while (await enumerator.MoveNextAsync(cancellationTokenSource.Token))
                    {
                        Trace.WriteLine(enumerator.Current);
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory(CategoryLinqCreate)]
        public async Task ExecutionIsDeferred()
        {
            var spy = new List<int>();
            var source = Create<int>(async (yield, cancellationToken) => 
            {
                spy.Clear();
                for (var i = 1; i < 10; i++)
                {
                    spy.Add(i);

                    await yield.ReturnAsync(i, cancellationToken);
                }
            });

            using (var enumerator = source.GetAsyncEnumerator())
            {
                await enumerator.MoveNextAsync();
                Trace.WriteLine(enumerator.Current);

                await enumerator.MoveNextAsync();
                Trace.WriteLine(enumerator.Current);
            }

            Assert.AreEqual(2, spy.Count);

            using (var enumerator = source.GetAsyncEnumerator())
            {
                await enumerator.MoveNextAsync();
                Trace.WriteLine(enumerator.Current);

                await enumerator.MoveNextAsync();
                Trace.WriteLine(enumerator.Current);
            }

            Assert.AreEqual(2, spy.Count);
        }


        [TestMethod]
        [TestCategory(CategoryLinqCreate)]
        public async Task ExecutionIsDeferredNested()
        {
            var spy = new List<int>();

            var source = Create<IAsyncEnumerable<int>>(async (y1, ct1) =>
            {
                spy.Clear();
                for (var i = 1; i < 10; i++)
                {
                    spy.Add(i);
                    var child = Create<int>(async (y2, ct2) =>
                    {
                        for (var j = 11; j < 20; j++)
                        {
                            spy.Add(j);
                            await y2.ReturnAsync(j, ct2);
                        }
                        y2.Break();
                    });

                    await y1.ReturnAsync(child, ct1);
                }
                y1.Break();
            });

            using (var outer = source.GetAsyncEnumerator())
            {
                await outer.MoveNextAsync();
                Trace.WriteLine(outer.Current);

                using (var inner = outer.Current.GetAsyncEnumerator())
                {
                    await inner.MoveNextAsync();
                    Trace.WriteLine(inner.Current);
                }
            }

            Assert.AreEqual(2, spy.Count);
        }
    }
}
