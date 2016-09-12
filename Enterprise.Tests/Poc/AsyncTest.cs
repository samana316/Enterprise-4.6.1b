using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Common.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enterprise.Tests.Poc
{
    [TestClass]
    public sealed class AsyncTest
    {
        private const string CategoryPocAsync = "Poc.Async";

        [TestMethod]
        [TestCategory(CategoryPocAsync)]
        [ExpectedException(typeof(NotImplementedException))]
        public async Task SimpleThrow()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory(CategoryPocAsync)]
        [ExpectedException(typeof(NotImplementedException))]
        public async Task SimpleThrowAsync()
        {
            await Task.Yield();

            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory(CategoryPocAsync)]
        [ExpectedException(typeof(NotImplementedException))]
        public async Task SimpleThrowTaskRun()
        {
            await Task.Run(() => { throw new NotImplementedException(); });
        }

        [TestMethod]
        [TestCategory(CategoryPocAsync)]
        [ExpectedException(typeof(NotImplementedException))]
        public async Task SimpleThrowTaskSource()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            taskCompletionSource.SetException(new NotImplementedException());

            await taskCompletionSource.Task;
        }

        [TestMethod]
        [TestCategory(CategoryPocAsync)]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task SimpleThrowTaskSourceAfterSet()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            taskCompletionSource.TrySetResult(true);
            taskCompletionSource.SetException(new NotImplementedException());

            await taskCompletionSource.Task;
        }

        //[TestMethod]
        [TestCategory(CategoryPocAsync)]
        public async Task InfiniteTaskRun()
        {
            ICollection<long> list = new List<long>();
            var current = 0L;

            try
            {
                Action action = () => this.InfiniteMethod(ref list, out current);

                using (var cancellationTokenSource = new CancellationTokenSource(1000))
                {
                    await this.InfiniteMethodWrapperAsync(action, cancellationTokenSource.Token);
                }
            }
            catch (OperationCanceledException exception)
            {
                Trace.WriteLine(exception);
            }

            Trace.WriteLine(list.Count, "Count");
            Trace.WriteLine(current, "Current");
            Trace.WriteLine(current - long.MinValue, "Progress");
        }

        [TestMethod]
        [TestCategory(CategoryPocAsync)]
        public async Task MultipleAwaits()
        {
            var closure = 0;
            var methodAsync = new Func<Task>(async () => 
            {
                closure++;

                await Console.Out.WriteLineAsync("WriteLineAsync: " + closure);
            });

            var task = methodAsync();
            await task;
            await task;
        }

        [TestMethod]
        [TestCategory(CategoryPocAsync)]
        [ExpectedException(typeof(NotImplementedException))]
        public async Task CustomAwaitable()
        {
            await new TestAwaitable();
        }

        private Task InfiniteMethodWrapperAsync(
            Action action,
            CancellationToken cancellationToken)
        {
            var task = Task.Run(action, cancellationToken);

            while (!cancellationToken.IsCancellationRequested)
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    break;
                }

                cancellationToken.ThrowIfCancellationRequested();
            }

            return task;
        }

        private void InfiniteMethod(
            ref ICollection<long> collection,
            out long current)
        {
            var i = long.MinValue;
            while (true)
            {
                if (i < long.MaxValue)
                {
                    i++;
                }
                else
                {
                    collection.Add(i);
                    i = long.MinValue;
                }

                current = i;
            }
        }

        private sealed class TestAwaitable : IAwaitable
        {
            public IAwaiter GetAwaiter()
            {
                throw new NotImplementedException();
            }
        }
    }
}
