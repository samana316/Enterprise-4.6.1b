using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Core.Reactive.Linq;
using Enterprise.Tests.Reactive.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enterprise.Tests.Poc
{
    [TestClass]
    public sealed class AsyncObservableTest
    {
        private const string CategoryAsyncObservable = "Poc.AsyncObservable";

        [TestMethod]
        [TestCategory(CategoryAsyncObservable)]
        public async Task SubscribeAsync()
        {
            var source = AsyncObservable.Create<int>(async (yield, cancellationToken) =>
            {
                await yield.ReturnAsync(1, cancellationToken);
                await yield.ReturnAsync(2, cancellationToken);
                await yield.ReturnAsync(3, cancellationToken);
            });

            var observer = new SpyAsyncObserver<int>();

            await source.SubscribeAsync(observer, CancellationToken.None);

            Assert.IsTrue(await observer.Items.SequenceEqualAsync(new[] { 1, 2, 3 }));
        }
    }
}
