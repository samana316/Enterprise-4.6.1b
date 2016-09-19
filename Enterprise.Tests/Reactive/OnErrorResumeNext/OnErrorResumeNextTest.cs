using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Core.Reactive.Linq;
using Enterprise.Tests.Reactive.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Enterprise.Core.Reactive.Linq.AsyncObservable;

namespace Enterprise.Tests.Reactive.OnErrorResumeNext
{
    [TestClass]
    public class OnErrorResumeNextTest
    {
        private const int DefaultTimeout = 1000;

        private const string CategoryReactiveOnErrorResumeNext = "Reactive.OnErrorResumeNext";

        [TestMethod]
        [TestCategory(CategoryReactiveOnErrorResumeNext)]
        public async Task Simple()
        {
            var first = Throw<int>(new InvalidOperationException());
            var second = Return(1);

            var query = first.OnErrorResumeNext(second);

            Assert.AreEqual(1, await query);
        }

        [TestMethod]
        [TestCategory(CategoryReactiveOnErrorResumeNext)]
        public async Task Chained()
        {
            var first = Throw<int>(new InvalidOperationException());
            var second = Return(1);
            var third = Throw<int>(new InvalidOperationException());
            var fourth = Return(2);

            var query = OnErrorResumeNext<int>(first, second, third, fourth);

            Assert.IsTrue(await query.SequenceEqual(new[] { 1, 2 }));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveOnErrorResumeNext)]
        [Timeout(DefaultTimeout)]
        public async Task Infinite()
        {
            using (var cancellationTokenSource = new CancellationTokenSource(500))
            {
                var cancellationToken = cancellationTokenSource.Token;

                var first = Throw<int>(new InvalidOperationException());
                var second = Repeat<int>(1).Take(2);
                var third = Throw<int>(new InvalidOperationException());
                var fourth = Return(2);

                var query = OnErrorResumeNext<int>(first, second, third, fourth).Repeat().Take(3);
                var observer = new SpyAsyncObserver<int>();
                await query.SubscribeAsync(observer, cancellationToken);

                Assert.IsTrue(await observer.Items.SequenceEqualAsync(new[] { 1, 1, 2 }, cancellationToken));
            }
        }
    }
}
