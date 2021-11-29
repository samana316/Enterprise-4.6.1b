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
        public async Task MultipleReturnAwaits()
        {
            var closure = 0;
            var methodAsync = new Func<Task<int>>(async () =>
            {
                closure++;

                await Console.Out.WriteLineAsync("WriteLineAsync: " + closure);

                return closure;
            });

            var task = methodAsync();
            Trace.WriteLine(await task, "await");
            Trace.WriteLine(await task, "await");
        }

        [TestMethod]
        [TestCategory(CategoryPocAsync)]
        [Timeout(1000)]
        public async Task WhenAnyInfinite()
        {
            var task1 = Task.Delay(500);
            var task2 = Task.Delay(int.MaxValue);

            await Task.WhenAny(task1, task2);
        }

        [TestMethod]
        [TestCategory(CategoryPocAsync)]
        [ExpectedException(typeof(NotImplementedException))]
        public async Task CustomAwaitable()
        {
            await new TestAwaitable();
        }

        [TestMethod]
        [TestCategory(CategoryPocAsync)]
        [Timeout(1000)]
        public async Task NotAsyncAllTheWay()
        {
            var result = await this.MethodDAsync();

            Trace.WriteLine(result);
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

        private async Task<int> MethodAAsync()
        {
            await Task.Delay(100);

            return 1;
        }

        private async Task<int> MethodBAsync()
        {
            await Task.Delay(200);

            return 2;
        }

        private int MethodC()
        {
            var x = MethodBAsync().Result;

            return x + 3;
        }

        private async Task<int> MethodDAsync()
        {
            var a = await this.MethodAAsync();
            var c = this.MethodC();

            return a + c;
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
