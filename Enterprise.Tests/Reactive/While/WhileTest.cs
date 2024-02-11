using System;
using System.Linq;
using System.Threading.Tasks;
using Enterprise.Core.Reactive;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Enterprise.Core.Reactive.Linq.AsyncObservable;

namespace Enterprise.Tests.Reactive.While
{
    [TestClass]
    public sealed class WhileTest
    {
        private const int DefaultTimeout = 1000;

        private const string CategoryReactiveWhile = "Reactive.While";

        [TestMethod]
        [TestCategory(CategoryReactiveWhile)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullSourceThrowsNullArgumentException()
        {
            IAsyncObservable<int> source = null;
            source.While(() => false);
        }

        [TestMethod]
        [TestCategory(CategoryReactiveWhile)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullConditionThrowsNullArgumentException()
        {
            var source = Empty<int>();
            Func<bool> condition = null;
            source.While(condition);
        }

        [TestMethod]
        [TestCategory(CategoryReactiveWhile)]
        [ExpectedException(typeof(NotImplementedException))]
        public async Task ExecutionIsDeferred()
        {
            var source = Throw<int>(new NotImplementedException());
            var result = source.While(() => true);

            await result;
        }

        [TestMethod]
        [TestCategory(CategoryReactiveWhile)]
        [Timeout(DefaultTimeout)]
        public async Task Simple()
        {
            var counter = 0;
            var source = Create<int>(async (yield, cancellationToken) =>
            {
                while (true)
                {
                    counter++;
                    await yield.ReturnAsync(counter, cancellationToken);
                }
            });

            var query = source.While(() => counter < 10);
            
            Assert.IsTrue(await query.SequenceEqual(Enumerable.Range(1, 9)));
        }
    }
}
