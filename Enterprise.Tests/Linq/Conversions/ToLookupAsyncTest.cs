using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Tests.Linq.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Enterprise.Core.Linq.AsyncEnumerable;

namespace Enterprise.Tests.Linq.ToLookupAsync
{
    [TestClass]
    public class ToLookupAsyncTest
    {
        private const string CategoryLinqToLookupAsync = "Linq.ToLookupAsync";

        [TestMethod]
        [TestCategory(CategoryLinqToLookupAsync)]
        [ExpectedException(typeof(NotImplementedException))]
        public async Task SourceSequenceIsReadEagerly()
        {
            var source = new ThrowAsyncEnumerable<int>();
            var lookup = await source.ToLookupAsync(x => x);
        }

        [TestMethod]
        [TestCategory(CategoryLinqToLookupAsync)]
        public async Task ChangesToSourceSequenceAfterToLookupAreNotNoticed()
        {
            var source = new List<string> { "abc" };
            var adapter = source.AsAsyncEnumerable();
            var lookup = await adapter.ToLookupAsync(x => x.Length);
            Assert.AreEqual(1, lookup.Count);

            // Potential new key is ignored
            source.Add("x");
            Assert.AreEqual(2, await adapter.CountAsync());
            Assert.AreEqual(1, lookup.Count);

            // Potential new value for existing key is ignored
            source.Add("xyz");
            Assert.AreEqual(3, await adapter.CountAsync());
            Assert.IsTrue(lookup[3].SequenceEqual(new[] { "abc" }));
        }

        [TestMethod]
        [TestCategory(CategoryLinqToLookupAsync)]
        public async Task LookupWithComparareAndElementSelector()
        {
            var people = new[] {
                new { First = "Jon", Last = "Skeet" },
                new { First = "Tom", Last = "SKEET" }, // Note upper-cased name
                new { First = "Juni", Last = "Cortez" },
                new { First = "Holly", Last = "Skeet" },
                new { First = "Abbey", Last = "Bartlet" },
                new { First = "Carmen", Last = "Cortez" },
                new { First = "Jed", Last = "Bartlet" }
            }.AsAsyncEnumerable();

            var lookup = await people.ToLookupAsync(p => p.Last, p => p.First, StringComparer.OrdinalIgnoreCase);
            Assert.IsTrue(lookup["Skeet"].SequenceEqual(new[] { "Jon", "Tom", "Holly" }));
            Assert.IsTrue(lookup["Cortez"].SequenceEqual(new[] { "Juni", "Carmen" }));
            // The key comparer is used for lookups too
            Assert.IsTrue(lookup["BARTLET"].SequenceEqual(new[] { "Abbey", "Jed" }));
            Assert.IsTrue(lookup.Select(x => x.Key).SequenceEqual(new[] { "Skeet", "Cortez", "Bartlet" }));
        }
    }
}
