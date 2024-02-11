using System;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Tests.Linq.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enterprise.Tests.Linq.ContainsAsync
{
    [TestClass]
    public sealed class ContainsAsyncTest
    {
        private const string CategoryLinqContainsAsync = "Linq.ContainsAsync";

        [TestMethod]
        [TestCategory(CategoryLinqContainsAsync)]
        public async Task MatchWithCustomComparer()
        {
            var source = new RealAsyncEnumerable<string>("foo", "bar", "baz");

            Assert.IsTrue(await source.ContainsAsync("BAR", StringComparer.OrdinalIgnoreCase));
        }
    }
}
