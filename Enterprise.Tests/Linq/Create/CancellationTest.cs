using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enterprise.Tests.Linq.Cancellation
{
    [TestClass]
    public sealed class CancellationTest
    {
        private const int DefaultTimeout = 1000;
        private const string CategoryLinqCancellation = "Linq.Cancellation";

        [TestMethod]
        [TestCategory(CategoryLinqCancellation)]
        [ExpectedException(typeof(OperationCanceledException))]
        public async Task SingleToken()
        {
            var source = AsyncEnumerable.Range(1, 3);

            using (var cts = new CancellationTokenSource())
            {
                var ct = cts.Token;

                using (var iterator = source.GetAsyncEnumerator())
                {
                    Assert.IsTrue(await iterator.MoveNextAsync(ct));
                    Assert.AreEqual(1, iterator.Current);

                    Assert.IsTrue(await iterator.MoveNextAsync(ct));
                    Assert.AreEqual(2, iterator.Current);

                    cts.Cancel();

                    await iterator.MoveNextAsync(ct);
                }
            }
        }

        [TestMethod]
        [TestCategory(CategoryLinqCancellation)]
        [ExpectedException(typeof(OperationCanceledException))]
        public async Task MultipleTokens()
        {
            var source = AsyncEnumerable.Range(1, 3);

            using (var cts = new CancellationTokenSource())
            {
                var ct = cts.Token;

                using (var iterator = source.GetAsyncEnumerator())
                {
                    Assert.IsTrue(await iterator.MoveNextAsync(ct));
                    Assert.AreEqual(1, iterator.Current);

                    Assert.IsTrue(await iterator.MoveNextAsync(ct));
                    Assert.AreEqual(2, iterator.Current);

                    cts.Cancel();

                    Assert.IsTrue(await iterator.MoveNextAsync());
                    Assert.IsFalse(await iterator.MoveNextAsync());

                    await iterator.MoveNextAsync(ct);
                }
            }
        }
    }
}
