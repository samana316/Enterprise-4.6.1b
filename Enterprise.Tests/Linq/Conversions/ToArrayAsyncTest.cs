using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Tests.Linq.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Enterprise.Core.Linq.AsyncEnumerable;

namespace Enterprise.Tests.Linq.ToArrayAsync
{
    [TestClass]
    public class ToArrayAsyncTest
    {
        private const string CategoryLinqToArrayAsync = "Linq.ToArrayAsync";

        [TestMethod]
        [TestCategory(CategoryLinqToArrayAsync)]
        public async Task SourceBothCollections()
        {
            var source = new[] { 1, 2, 3 };
            var query = new ListAsyncEnumerable<int>(source);
            var array = await query.ToArrayAsync();

            CollectionAssert.AreEquivalent(source, array);

            query.Add(4);
            CollectionAssert.AreNotEquivalent(query, array);
        }

        [TestMethod]
        [TestCategory(CategoryLinqToArrayAsync)]
        public async Task SourceGenericCollectionOnly()
        {
            var source = new HashSet<int>
            {
                1,2,3
            };

            var query = source.AsAsyncEnumerable();
            var array = await query.ToArrayAsync();

            Assert.IsTrue(array.SequenceEqual(source));
        }

        [TestMethod]
        [TestCategory(CategoryLinqToArrayAsync)]
        public async Task SourceStreamed()
        {
            var source = new RealAsyncEnumerable<int>(1, 2, 3);
            var array = await source.ToArrayAsync();

            Assert.IsTrue(await source.SequenceEqualAsync(array));
        }

        [TestMethod]
        [TestCategory(CategoryLinqToArrayAsync)]
        public async Task ResultIsIndependentOfSource()
        {
            var source = new ListAsyncEnumerable<string> { "xyz", "abc" };
            var result = await source.ToArrayAsync();
            CollectionAssert.AreEquivalent(source, result);

            // Change the source: result won’t have changed
            source[0] = "xxx";
            Assert.AreEqual("xyz", result[0]);

            // And the reverse
            result[1] = "yyy";
            Assert.AreEqual("abc", source[1]);
        }
    }
}
