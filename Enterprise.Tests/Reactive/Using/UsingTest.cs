using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Enterprise.Core.Common;
using Enterprise.Core.Linq;
using Enterprise.Core.Reactive.Linq;
using Enterprise.Tests.Reactive.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Enterprise.Core.Reactive.Linq.AsyncObservable;

namespace Enterprise.Tests.Reactive.Using
{
    [TestClass]
    public class UsingTest
    {
        private const int DefaultTimeout = 1000;

        private const string CategoryReactiveUsing = "Reactive.Using";

        [TestMethod]
        [TestCategory(CategoryReactiveUsing)]
        [Timeout(DefaultTimeout)]
        public async Task Simple()
        {
            var disposable = new SpyDisposable();
            var source = AsyncObservable.Interval(TimeSpan.FromMilliseconds(10));
            var result = AsyncObservable.Using(
                () => disposable,
                test => source);

            var observer = new SpyAsyncObserver<long>();
            await result.Take(5).SubscribeAsync(observer);

            Assert.IsTrue(await observer.Items.SequenceEqualAsync(new long[] { 0, 1, 2, 3, 4 }));
            Assert.IsTrue(disposable.IsDisposed);
        }

        [TestMethod]
        [TestCategory(CategoryReactiveUsing)]
        [Timeout(DefaultTimeout)]
        public async Task Error()
        {
            var disposable = new SpyDisposable();
            var source = AsyncObservable.Interval(TimeSpan.FromMilliseconds(10));
            var result = AsyncObservable.Using(
                () => disposable,
                test => source);

            var observer = new SpyAsyncObserver<long>();
            await result.Take(5).Concat(Throw<long>(new NotImplementedException())).SubscribeAsync(observer);

            Assert.IsTrue(await observer.Items.SequenceEqualAsync(new long[] { 0, 1, 2, 3, 4 }));
            Assert.IsTrue(disposable.IsDisposed);
            Assert.IsTrue(observer.Error.InnerExceptions.Any());
        }

        [TestMethod]
        [TestCategory(CategoryReactiveUsing)]
        [Timeout(DefaultTimeout)]
        public async Task SimpleAsync()
        {
            var disposable = new SpyDisposable();
            var source = AsyncObservable.Interval(TimeSpan.FromMilliseconds(10));
            var result = AsyncObservable.Using(
                () => disposable,
                (test, cancellationToken) => Task.FromResult(source));

            var observer = new SpyAsyncObserver<long>();
            await result.Take(5).SubscribeAsync(observer);

            Assert.IsTrue(await observer.Items.SequenceEqualAsync(new long[] { 0, 1, 2, 3, 4 }));
            Assert.IsTrue(disposable.IsDisposed);
        }

        private sealed class SpyDisposable : DisposableBase
        {
            public bool IsDisposed { get; private set; }

            protected override void Dispose(
                bool disposing)
            {
                Trace.WriteLine(disposing, "Dispose");
                this.IsDisposed = true;

                base.Dispose(disposing);
            }
        }
    }
}
