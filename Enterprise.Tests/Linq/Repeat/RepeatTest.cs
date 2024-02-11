using System;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enterprise.Tests.Linq.Repeat
{
    [TestClass]
    public class RepeatTest
    {
        private const string CategoryLinqRepeat = "Linq.Repeat";

        [TestMethod]
        [TestCategory(CategoryLinqRepeat)]
        public async Task Simple()
        {
            var source = AsyncEnumerable.Repeat(1, 3);
            Assert.IsTrue(await source.SequenceEqualAsync(new[] { 1, 1, 1 }));
        }

        [TestMethod]
        [TestCategory(CategoryLinqRepeat)]
        public async Task Empty()
        {
            var source = AsyncEnumerable.Repeat(1, 0);
            Assert.IsTrue(await source.SequenceEqualAsync(new int[0]));
        }

        [TestMethod]
        [TestCategory(CategoryLinqRepeat)]
        public async Task Null()
        {
            var source = AsyncEnumerable.Repeat<object>(null, 3);
            Assert.IsTrue(await source.SequenceEqualAsync(new object[] { null, null, null }));
        }

        [TestMethod]
        [TestCategory(CategoryLinqRepeat)]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CountCantBeNegative()
        {
            var source = AsyncEnumerable.Repeat(int.MinValue, -1);
        }
    }
}
