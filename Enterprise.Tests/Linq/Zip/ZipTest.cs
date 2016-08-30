using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Tests.Linq.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Enterprise.Core.Linq.AsyncEnumerable;

namespace Enterprise.Tests.Linq.Zip
{
    [TestClass]
    public sealed class ZipTest
    {
        private const string CategoryLinqZip = "Linq.Zip";

        [TestMethod]
        [TestCategory(CategoryLinqZip)]
        public async Task AdjacentElements()
        {
            var elements = new RealAsyncEnumerable<string>("a", "b", "c", "d", "e");
            var query = elements.Zip(elements.Skip(1), (x, y) => x + y);

            Assert.IsTrue(await query.SequenceEqualAsync(new[] { "ab", "bc", "cd", "de" }));
        }
    }
}
