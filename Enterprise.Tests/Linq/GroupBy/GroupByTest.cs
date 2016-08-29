using System;
using System.Linq;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Tests.Linq.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Enterprise.Core.Linq.AsyncEnumerable;

namespace Enterprise.Tests.Linq.GroupBy
{
    [TestClass]
    public sealed class GroupByTest
    {
        private const string CategoryLinqGroupBy = "Linq.GroupBy";

        [TestMethod]
        [TestCategory(CategoryLinqGroupBy)]
        public void ExecutionIsPartiallyDeferred()
        {
            var source = new ThrowAsyncEnumerable<int>();
            var result = from x in source group x by x;
        }

        [TestMethod]
        [TestCategory(CategoryLinqGroupBy)]
        [ExpectedException(typeof(DivideByZeroException))]
        public async Task SequenceIsReadFullyBeforeFirstResultReturned()
        {
            var numbers = new RealAsyncEnumerable<int>(1, 2, 3, 4, 5, 6, 7, 8, 9, 0);

            var groups =
                from number in numbers
                group number by 10 / number;

            using (var iterator = groups.GetAsyncEnumerator())
            {
                await iterator.MoveNextAsync();
            }
        }

        [TestMethod]
        [TestCategory(CategoryLinqGroupBy)]
        public async Task GroupByWithElementProjectionAndCollectionProjection()
        {
            var source = new RealAsyncEnumerable<string>("abc", "hello", "def", "there", "four");

            var groups =
                from item in source
                group item[0] by item.Length into values
                select values.Key + ":" + string.Join(";", values);

            Assert.IsTrue(await groups.SequenceEqualAsync(new[] { "3:a;d", "5:h;t", "4:f" }));
        }
    }
}
