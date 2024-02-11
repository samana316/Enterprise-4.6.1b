using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Tests.Linq.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enterprise.Tests.Linq.Paging
{
    [TestClass]
    public sealed class PagingTest
    {
        private const string CategoryLinqPaging = "Linq.Paging";

        [TestMethod]
        [TestCategory(CategoryLinqPaging)]
        public void ExecutionIsDeferred()
        {
            var source = new ThrowAsyncEnumerable<string>();
            var query = source.Take(1).Skip(1).TakeWhile(string.IsNullOrEmpty).SkipWhile(string.IsNullOrEmpty);
        }

        [TestMethod]
        [TestCategory(CategoryLinqPaging)]
        public async Task PagingSimple()
        {
            var source = new RealAsyncEnumerable<int>(Enumerable.Range(0, 4)).AsAsyncEnumerable();
            source = source.Concat(source).OrderBy(x => x);

            var itemsPerPage = 2;

            for (var pageNumber = 0; pageNumber < 4; pageNumber++)
            {
                var pageItems = source
                    .Skip(pageNumber * itemsPerPage)
                    .Take(itemsPerPage);
                
                var expected = Enumerable.Repeat(pageNumber, itemsPerPage);

                Assert.IsTrue(await pageItems.SequenceEqualAsync(expected));
            }
        }
    }
}