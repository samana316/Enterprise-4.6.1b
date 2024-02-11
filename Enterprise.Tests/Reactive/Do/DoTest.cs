using System;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Core.Reactive;
using Enterprise.Core.Reactive.Linq;
using Enterprise.Tests.Reactive.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enterprise.Tests.Reactive.Do
{
    [TestClass]
    public sealed class DoTest
    {
        private const int DefaultTimeout = 1000;

        private const string CategoryReactiveDo = "Reactive.Do";

        [TestMethod]
        [TestCategory(CategoryReactiveDo)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullSourceThrowsNullArgumentException()
        {
            IAsyncObservable<int> source = null;
            source.Do(default(IAsyncObserver<int>));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveDo)]
        public async Task Simple()
        {
            var source = AsyncObservable.Range(1, 3);
            var observer = new SpyAsyncObserver<int>();

            var query = source.Do(observer);
            await query;

            Assert.IsTrue(await observer.Items.SequenceEqualAsync(new[] { 1, 2, 3 }));
            Assert.IsTrue(observer.IsCompleted);
        }
    }
}
