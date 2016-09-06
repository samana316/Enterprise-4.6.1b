using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Tests.Linq.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Enterprise.Core.Linq.AsyncEnumerable;

namespace Enterprise.Tests.Linq.ToDictionaryAsync
{
    [TestClass]
    public class ToDictionaryAsyncTest
    {
        private const string CategoryLinqToDictionaryAsync = "Linq.ToDictionaryAsync";

        [TestMethod]
        [TestCategory(CategoryLinqToDictionaryAsync)]
        public async Task Simple()
        {
            var source = new RealAsyncEnumerable<string>("Address", "Binding", "Contract");
            var dictionary = await source.ToDictionaryAsync(x => x[0]);

            Assert.IsTrue(dictionary.Keys.SequenceEqual("ABC"));
            Assert.IsTrue(await source.SequenceEqualAsync(dictionary.Values));
        }

        [TestMethod]
        [TestCategory(CategoryLinqToDictionaryAsync)]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task NullKeys()
        {
            var source = new RealAsyncEnumerable<string>(null);
            var dictionary = await source.ToDictionaryAsync(x => x);
        }

        [TestMethod]
        [TestCategory(CategoryLinqToDictionaryAsync)]
        public async Task NullValues()
        {
            var source = new Dictionary<int, string>
            {
                { 1, "A" },
                { 2, "B" },
                { 3, "C" },
                { 4, null },
                { 5, "Z" }
            }.AsAsyncEnumerable();

            var dictionary = await source.ToDictionaryAsync(x => x.Key, x => x.Value);

            Assert.IsTrue(await source.SequenceEqualAsync(dictionary));
        }

        [TestMethod]
        [TestCategory(CategoryLinqToDictionaryAsync)]
        [ExpectedException(typeof(ArgumentException))]
        public async Task DuplicateKeys()
        {
            var source = new RealAsyncEnumerable<string>("Address", "Binding", "Contract", "about");
            var dictionary = await source.ToDictionaryAsync(
                x => x[0].ToString(), StringComparer.OrdinalIgnoreCase);
        }
    }
}
