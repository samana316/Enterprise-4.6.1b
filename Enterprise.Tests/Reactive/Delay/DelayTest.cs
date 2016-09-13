using System;
using System.Linq;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Core.Reactive;
using Enterprise.Core.Reactive.Linq;
using Enterprise.Tests.Reactive.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enterprise.Tests.Reactive.Delay
{
    [TestClass]
    public sealed class DelayTest
    {
        private const int DefaultTimeout = 1000;

        private const string CategoryReactiveDelay = "Reactive.Delay";

        [TestMethod]
        [TestCategory(CategoryReactiveDelay)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullSourceThrowsNullArgumentException()
        {
            IAsyncObservable<int> source = null;
            source.Delay(TimeSpan.Zero);
        }

        [TestMethod]
        [TestCategory(CategoryReactiveDelay)]
        public async Task WithTimeSpan()
        {
            var source = AsyncObservable.Range(1, 3);
            var observer = new SpyAsyncObserver<int>();

            var query = source.Delay(TimeSpan.FromMilliseconds(10));
            await query.SubscribeAsync(observer);

            Assert.IsTrue(await observer.Items.SequenceEqualAsync(new[] { 1, 2, 3 }));
            Assert.IsTrue(observer.IsCompleted);
        }

        [TestMethod]
        [TestCategory(CategoryReactiveDelay)]
        public async Task WithValidDateTimeOffset()
        {
            var source = AsyncObservable.Range(1, 3);
            var observer = new SpyAsyncObserver<int>();

            var query = source.Delay(DateTimeOffset.Now.AddMilliseconds(10));
            await query.SubscribeAsync(observer);

            Assert.IsTrue(await observer.Items.SequenceEqualAsync(new[] { 1, 2, 3 }));
            Assert.IsTrue(observer.IsCompleted);
        }

        [TestMethod]
        [TestCategory(CategoryReactiveDelay)]
        public async Task WithInvalidDateTimeOffset()
        {
            var source = AsyncObservable.Range(1, 3);
            var observer = new SpyAsyncObserver<int>();

            var query = source.Delay(DateTimeOffset.Now.AddMilliseconds(-10));
            await query.SubscribeAsync(observer);
            
            Assert.IsTrue(observer.Error.InnerExceptions.Any());
            Assert.IsTrue(observer.IsCompleted);
        }
    }
}
