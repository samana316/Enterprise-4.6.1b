using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Core.Reactive.Linq;
using Enterprise.Tests.Reactive.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Enterprise.Core.Reactive.Linq.AsyncObservable;

namespace Enterprise.Tests.Reactive.Finally
{
    [TestClass]
    public class FinallyTest
    {
        private const int DefaultTimeout = 1000;

        private const string CategoryReactiveFinally = "Reactive.Finally";

        [TestMethod]
        [TestCategory(CategoryReactiveFinally)]
        public async Task Success()
        {
            var isFinallyCalled = false;
            var source = AsyncObservable.Range(1, 3);
            var query = source.Finally(() => 
            {
                Trace.WriteLine("Finally");
                isFinallyCalled = true;
            });

            var observer = new SpyAsyncObserver<int>();
            await query.SubscribeAsync(observer);

            Assert.IsTrue(await observer.Items.SequenceEqualAsync(new[] { 1, 2, 3 }));
            Assert.IsTrue(isFinallyCalled);
        }

        [TestMethod]
        [TestCategory(CategoryReactiveFinally)]
        public async Task Error()
        {
            var isFinallyCalled = false;
            var source = AsyncObservable.Range(1, 3).Concat(Throw<int>(new NotImplementedException()));
            var query = source.Finally(() =>
            {
                Trace.WriteLine("Finally");
                isFinallyCalled = true;
            });

            var observer = new SpyAsyncObserver<int>();
            await query.SubscribeAsync(observer);

            Assert.IsTrue(await observer.Items.SequenceEqualAsync(new[] { 1, 2, 3 }));
            Assert.IsTrue(observer.Error.InnerExceptions.Any());
            Assert.IsTrue(isFinallyCalled);
        }
    }
}
