using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Core.Reactive.Linq;
using Enterprise.Tests.Reactive.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Enterprise.Core.Reactive.Linq.AsyncObservable;

namespace Enterprise.Tests.Reactive.Retry
{
    [TestClass]
    public sealed class RetryTest
    {
        private const int DefaultTimeout = 1000;

        private const string CategoryReactiveRetry = "Reactive.Retry";

        [TestMethod]
        [TestCategory(CategoryReactiveRetry)]
        [Timeout(DefaultTimeout)]
        public async Task Simple()
        {
            var i = 0;
            var source = Create<int>(async (yield, cancellationToken) => 
            {
                i++;
                if (i == 1)
                {
                    await yield.ReturnAllAsync(new[] { 1, 2 }, cancellationToken);
                    DivideByZero.Instance();
                }
                else if (i == 2)
                {
                    await yield.ReturnAllAsync(new[] { 1, 2, 3 }, cancellationToken);
                    DivideByZero.Instance();
                }
                else
                {
                    await yield.ReturnAsync(1, cancellationToken);
                }
            });

            var query = source.Retry();
            var observer = query.CreateSpyAsyncObserver();
            await query.SubscribeAsync(observer, CancellationToken.None);

            Assert.IsTrue(observer.IsCompleted);
            Assert.IsFalse(observer.Error.InnerExceptions.Any());
            Assert.IsTrue(await observer.Items.SequenceEqualAsync(new[] { 1, 2, 1, 2, 3, 1 }));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveRetry)]
        [Timeout(DefaultTimeout)]
        public async Task SpecifyCount()
        {
            var i = 0;
            var source = Create<int>(async (yield, cancellationToken) =>
            {
                i++;
                if (i == 1)
                {
                    await yield.ReturnAllAsync(new[] { 0, 1, 2 }, cancellationToken);
                    DivideByZero.Instance();
                }
                else if (i == 2)
                {
                    await yield.ReturnAllAsync(new[] { 0, 1, 2 }, cancellationToken);
                    DivideByZero.Instance();
                }
                else
                {
                    await yield.ReturnAsync(0, cancellationToken);
                }
            });

            var query = source.Retry(2);
            var observer = query.CreateSpyAsyncObserver();
            await query.SubscribeAsync(observer, CancellationToken.None);

            Assert.IsTrue(observer.IsCompleted);
            Assert.IsTrue(observer.Error.InnerExceptions.Any());
            Assert.IsTrue(await observer.Items.SequenceEqualAsync(new[] { 0, 1, 2, 0, 1, 2 }));
        }
    }
}
