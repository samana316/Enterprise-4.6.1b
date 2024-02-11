using System;
using System.Linq;
using System.Threading.Tasks;
using Enterprise.Core.Reactive;
using Enterprise.Core.Reactive.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Enterprise.Core.Reactive.Linq.AsyncObservable;

namespace Enterprise.Tests.Reactive.DoWhile
{
    [TestClass]
    public sealed class DoWhileTest
    {
        private const int DefaultTimeout = 1000;

        private const string CategoryReactiveDoWhile = "Reactive.DoWhile";

        [TestMethod]
        [TestCategory(CategoryReactiveDoWhile)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullSourceThrowsNullArgumentException()
        {
            IAsyncObservable<int> source = null;
            source.DoWhile(() => false);
        }

        [TestMethod]
        [TestCategory(CategoryReactiveDoWhile)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullConditionThrowsNullArgumentException()
        {
            var source = Empty<int>();
            Func<bool> condition = null;
            source.DoWhile(condition);
        }

        [TestMethod]
        [TestCategory(CategoryReactiveDoWhile)]
        [ExpectedException(typeof(NotImplementedException))]
        public async Task ExecutionIsDeferred()
        {
            var source = Throw<int>(new NotImplementedException());
            var result = source.DoWhile(() => true);

            await result;
        }

        [TestMethod]
        [TestCategory(CategoryReactiveDoWhile)]
        [Timeout(DefaultTimeout)]
        public async Task Simple()
        {
            var source = AsyncObservable.Range(1, 10);
            Func<bool> alwaysFalse = () => false;

            var query1 = source.While(alwaysFalse);
            var query2 = source.DoWhile(alwaysFalse);

            Assert.IsTrue(await query1.SequenceEqual(new int[0]));
            Assert.IsTrue(await query2.SequenceEqual(new int[] { 1 }));
        }
    }
}
