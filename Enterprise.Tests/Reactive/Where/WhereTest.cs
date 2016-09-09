using System;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Core.Reactive;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Enterprise.Core.Reactive.Linq.AsyncObservable;

namespace Enterprise.Tests.Reactive.Where
{
    [TestClass]
    public sealed class WhereTest
    {
        private const string CategoryReactiveWhere = "Reactive.Where";

        [TestMethod]
        [TestCategory(CategoryReactiveWhere)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullSourceThrowsNullArgumentException()
        {
            IAsyncObservable<int> source = null;
            source.Where(x => x > 5);
        }

        [TestMethod]
        [TestCategory(CategoryReactiveWhere)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullPredicateThrowsNullArgumentException()
        {
            var source = Empty<int>();
            Func<int, bool> predicate = null;
            source.Where(predicate);
        }

        [TestMethod]
        [TestCategory(CategoryReactiveWhere)]
        [ExpectedException(typeof(NotImplementedException))]
        public async Task ExecutionIsDeferred()
        {
            var source = Throw<int>(new NotImplementedException());
            var result = source.Where(x => x > 0);

            await result;
        }

        [TestMethod]
        [TestCategory(CategoryReactiveWhere)]
        public async Task QueryExpressionSimpleFiltering()
        {
            var source = new[] { 1, 3, 4, 2, 8, 1 }.ToAsyncObservable();

            var result =
                from x in source
                where x < 4
                select x;

            Assert.IsTrue(await result.SequenceEqual(new[] { 1, 3, 2, 1 }));
        }
    }
}
