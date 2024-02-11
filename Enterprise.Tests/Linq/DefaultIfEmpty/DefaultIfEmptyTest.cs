using System.Linq;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Tests.Linq.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Enterprise.Core.Linq.AsyncEnumerable;

namespace Enterprise.Tests.Linq.DefaultIfEmpty
{
    [TestClass]
    public class DefaultIfEmptyTest
    {
        private const string CategoryLinqDefaultIfEmpty = "Linq.DefaultIfEmpty";

        [TestMethod]
        [TestCategory(CategoryLinqDefaultIfEmpty)]
        public async Task EmptyNoDefault()
        {
            var source = Empty<int>();
            var query = source.DefaultIfEmpty();

            Assert.IsTrue(await query.SequenceEqualAsync(new[] { 0 }));
        }

        [TestMethod]
        [TestCategory(CategoryLinqDefaultIfEmpty)]
        public async Task EmptyWithDefault()
        {
            var source = Empty<int>();
            var query = source.DefaultIfEmpty(1);

            Assert.IsTrue(await query.SequenceEqualAsync(new[] { 1 }));
        }

        [TestMethod]
        [TestCategory(CategoryLinqDefaultIfEmpty)]
        public async Task NonEmptyNoDefault()
        {
            var source = new RealAsyncEnumerable<int>(1, 2, 3);
            var query = source.DefaultIfEmpty();

            Assert.IsTrue(await query.SequenceEqualAsync(new[] { 1, 2, 3 }));
        }

        [TestMethod]
        [TestCategory(CategoryLinqDefaultIfEmpty)]
        public async Task NonEmptyWithDefault()
        {
            var source = new RealAsyncEnumerable<int>(1, 2, 3);
            var query = source.DefaultIfEmpty(1);

            Assert.IsTrue(await query.SequenceEqualAsync(new[] { 1, 2, 3 }));
        }
    }
}
