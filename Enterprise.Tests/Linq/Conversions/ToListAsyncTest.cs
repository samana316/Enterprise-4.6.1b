using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Tests.Linq.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Enterprise.Core.Linq.AsyncEnumerable;

namespace Enterprise.Tests.Linq.ToListAsync
{
    [TestClass]
    public class ToListAsyncTest
    {
        private const string CategoryLinqToListAsync = "Linq.ToListAsync";

        [TestMethod]
        [TestCategory(CategoryLinqToListAsync)]
        public async Task SourceBothCollections()
        {
            var source = new[] { 1, 2, 3 };
            var query = new ListAsyncEnumerable<int>(source);
            var list = await query.ToListAsync();

            CollectionAssert.AreEquivalent(source, list);

            query.Add(4);
            CollectionAssert.AreNotEquivalent(query, list);
        }

        [TestMethod]
        [TestCategory(CategoryLinqToListAsync)]
        public async Task SourceGenericCollectionOnly()
        {
            var source = new HashSet<int>
            {
                1,2,3
            };

            var query = source.AsAsyncEnumerable();
            var list = await query.ToListAsync();

            Assert.IsTrue(list.SequenceEqual(source));
        }

        [TestMethod]
        [TestCategory(CategoryLinqToListAsync)]
        public async Task SourceStreamed()
        {
            var source = new RealAsyncEnumerable<int>(1, 2, 3);
            var list = await source.ToListAsync();

            Assert.IsTrue(await source.SequenceEqualAsync(list));
        }
    }
}
