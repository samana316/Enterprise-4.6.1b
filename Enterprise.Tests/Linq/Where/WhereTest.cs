using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Tests.Linq.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enterprise.Tests.Linq.Where
{
    [TestClass]
    public sealed class WhereTest
    {
        private const string CategoryLinqWhere = "Linq.Where";

        [TestMethod]
        [TestCategory(CategoryLinqWhere)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullSourceThrowsNullArgumentException()
        {
            IAsyncEnumerable<int> source = null;
            source.Where(x => x > 5);
        }

        [TestMethod]
        [TestCategory(CategoryLinqWhere)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullPredicateThrowsNullArgumentException()
        {
            var source = new DummyAsyncEnumerable<int>();
            Func<int, bool> predicate = null;
            source.Where(predicate);
        }

        [TestMethod]
        [TestCategory(CategoryLinqWhere)]
        [Timeout(60000)]
        [ExpectedException(typeof(NotImplementedException))]
        public async Task ExecutionIsDeferred()
        {
            var source = new ThrowAsyncEnumerable<int>();
            var result = source.Where(x => x > 0);

            using (var iterator = result.GetAsyncEnumerator())
            {
                while (await iterator.MoveNextAsync())
                {
                    Trace.WriteLine(iterator.Current);
                }
            }
        }

        [TestMethod]
        [TestCategory(CategoryLinqWhere)]
        public async Task QueryExpressionSimpleFiltering()
        {
            var source = new RealAsyncEnumerable<int>(1, 3, 4, 2, 8, 1);
            var result =
                from x in source
                where x < 4
                select x;

            Assert.IsTrue(await result.SequenceEqualAsync(new[] { 1, 3, 2, 1 }));
        }
    }
}
