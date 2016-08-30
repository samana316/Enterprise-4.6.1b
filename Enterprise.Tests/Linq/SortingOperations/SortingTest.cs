using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Tests.Linq.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Enterprise.Core.Linq.AsyncEnumerable;

namespace Enterprise.Tests.Linq.Sorting
{
    [TestClass]
    public sealed class SortingTest
    {
        private const string CategoryLinqSorting = "Linq.Sorting";

        [TestMethod]
        [TestCategory(CategoryLinqSorting)]
        public void ExecutionIsDeferred()
        {
            var source = new ThrowAsyncEnumerable<string>();

            var query =
                from item in source
                orderby item.Length
                select item;
        }

        [TestMethod]
        [TestCategory(CategoryLinqSorting)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateSource()
        {
            var source = default(IAsyncEnumerable<string>);

            var query =
                from item in source
                orderby item.Length
                select item;
        }

        [TestMethod]
        [TestCategory(CategoryLinqSorting)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateSelector()
        {
            var source = new DummyAsyncEnumerable<string>();

            var query = source.OrderBy(default(Func<string, int>));
        }

        [TestMethod]
        [TestCategory(CategoryLinqSorting)]
        public async Task OrderingIsStable()
        {
            var source = new[]
            {
                new { Value = 1, Key = 10 },
                new { Value = 2, Key = 11 },
                new { Value = 3, Key = 11 },
                new { Value = 4, Key = 10 },
            }.AsAsyncEnumerable();

            var query =
                from item in source
                orderby item.Key
                select item.Value;

            Assert.IsTrue(await query.SequenceEqualAsync(new[] { 1, 4, 2, 3 }));
        }

        [TestMethod]
        [TestCategory(CategoryLinqSorting)]
        public async Task OrderingMultipleKeys()
        {
            var source = new[]
            {
                new { Value = 1, Key = 1 },
                new { Value = 1, Key = 2 },
                new { Value = 2, Key = 3 },
                new { Value = 2, Key = 4 },
            }.AsAsyncEnumerable();

            var query =
                from item in source
                orderby item.Value descending, item.Key
                select item.Key;

            Assert.IsTrue(await query.SequenceEqualAsync(new[] { 3, 4, 1, 2 }));
        }

        [TestMethod]
        [TestCategory(CategoryLinqSorting)]
        public async Task CustomComparer()
        {
            var source = new RealAsyncEnumerable<double>(1.3, 3.5, 6.3, 3.1);
            var comparer = Comparer<double>.Create((x, y) => Comparer<int>.Default.Compare((int)x, (int)y));

            var query = source.OrderBy(x => x, comparer);

            Assert.IsTrue(await query.SequenceEqualAsync(new[] { 1.3, 3.5, 3.1, 6.3 }));
        }
    }
}
