using System.Collections.Generic;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Tests.Linq.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Enterprise.Core.Linq.AsyncEnumerable;

namespace Enterprise.Tests.Linq.CountAsync
{
    [TestClass]
    public sealed class CountAsyncTest
    {
        private const string CategoryLinqCountAsync = "Linq.CountAsync";

        [TestMethod]
        [TestCategory(CategoryLinqCountAsync)]
        public async Task SourceBothCollections()
        {
            var source = new ListAsyncEnumerable<int> { 1, 2, 3 };
            Assert.AreEqual(source.Count, await source.CountAsync());
            Assert.AreEqual(source.Count, await source.LongCountAsync());
        }

        [TestMethod]
        [TestCategory(CategoryLinqCountAsync)]
        public async Task SourceGenericCollectionOnly()
        {
            var source = new HashSet<int>
            {
                1,2,3
            };

            var query = source.AsAsyncEnumerable();
            Assert.AreEqual(source.Count, await query.CountAsync());

            source.Add(4);
            Assert.AreEqual(source.Count, await query.LongCountAsync());
            Assert.AreEqual(3, await query.CountAsync(x => x > 1));
        }

        [TestMethod]
        [TestCategory(CategoryLinqCountAsync)]
        public async Task SourceNonGenericCollectionOnly()
        {
            var source = new SemiGenericAsyncCollection<int>(1, 2, 3);

            var query = source.AsAsyncEnumerable();
            Assert.AreEqual(source.Count, await query.CountAsync());

            source.Add(4);
            Assert.AreEqual(source.Count, await query.LongCountAsync());
        }

        [TestMethod]
        [TestCategory(CategoryLinqCountAsync)]
        public async Task SourceStreamed()
        {
            var source = new RealAsyncEnumerable<int>(1, 2, 3);
            
            Assert.AreEqual(3, await source.CountAsync());
            Assert.AreEqual(3, await source.LongCountAsync());
            Assert.AreEqual(2, await source.CountAsync(x => x > 1));
            Assert.AreEqual(1, await source.LongCountAsync(x => x > 2));
        }
    }
}
