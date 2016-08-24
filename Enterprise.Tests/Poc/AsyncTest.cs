using System;
using System.Threading.Tasks;
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
    }
}
