using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Reactive.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enterprise.Tests.Reactive.Paging
{
    [TestClass]
    public class PagingTest
    {
        private const int DefaultTimeout = 1000;

        private const string CategoryReactivePaging = "Reactive.Paging";

        [TestMethod]
        [TestCategory(CategoryReactivePaging)]
        public async Task TakeSimple()
        {
            var source = AsyncObservable.Range(1, int.MaxValue);
            var query = source.Take(3);

            Assert.IsTrue(await query.SequenceEqual(Enumerable.Range(1, 3)));
        }

        [TestMethod]
        [TestCategory(CategoryReactivePaging)]
        public async Task TakeWhileSimple()
        {
            var source = AsyncObservable.Range(1, int.MaxValue);
            var query = source.TakeWhile(x => x < 10);

            Assert.IsTrue(await query.SequenceEqual(Enumerable.Range(1, 9)));
        }

        [TestMethod]
        [TestCategory(CategoryReactivePaging)]
        public async Task SkipSimple()
        {
            var source = AsyncObservable.Range(1, 5);
            var query = source.Skip(3);

            Assert.IsTrue(await query.SequenceEqual(Enumerable.Range(4, 3)));
        }

        [TestMethod]
        [TestCategory(CategoryReactivePaging)]
        public async Task SkipWhileSimple()
        {
            var source = AsyncObservable.Range(-5, 10);
            var query = source.SkipWhile(x => x < 0);

            Assert.IsTrue(await query.SequenceEqual(Enumerable.Range(0, 4)));
        }
    }
}
