using System;
using System.Linq;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Tests.Linq.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Enterprise.Core.Linq.AsyncEnumerable;

namespace Enterprise.Tests.Linq.SumAsync
{
    [TestClass]
    public class SumAsyncTest
    {
        private const string CategoryLinqSumAsync = "Linq.SumAsync";

        [TestMethod]
        [TestCategory(CategoryLinqSumAsync)]
        public async Task Simple()
        {
            var expected = new[] { 1, 2, 3 };
            var source = new RealAsyncEnumerable<int>(expected);

            Assert.AreEqual(expected.Sum(), await source.SumAsync());
        }

        [TestMethod]
        [TestCategory(CategoryLinqSumAsync)]
        public async Task Empty()
        {
            var source = Empty<long?>();

            Assert.AreEqual(source.Sum(), await source.SumAsync());
        }

        [TestMethod]
        [TestCategory(CategoryLinqSumAsync)]
        public async Task SimpleWithOnlyNull()
        {
            var expected = new float?[] { null, null, null, null };
            var source = new RealAsyncEnumerable<float?>(expected);

            Assert.AreEqual(expected.Sum(), await source.SumAsync());
        }

        [TestMethod]
        [TestCategory(CategoryLinqSumAsync)]
        public async Task SimpleWithSomeNull()
        {
            var expected = new double?[] { 1.1, 2.2, null, 3.3, null };
            var source = new RealAsyncEnumerable<double?>(expected);

            Assert.AreEqual(expected.Sum(), await source.SumAsync());
        }

        [TestMethod]
        [TestCategory(CategoryLinqSumAsync)]
        [ExpectedException(typeof(OverflowException))]
        public async Task PossitiveOverflow()
        {
            var expected = new decimal[] { decimal.MaxValue, decimal.MaxValue };
            var source = new RealAsyncEnumerable<decimal>(expected);

            Assert.AreEqual(expected.Sum(), await source.SumAsync());
        }

        [TestMethod]
        [TestCategory(CategoryLinqSumAsync)]
        [ExpectedException(typeof(OverflowException))]
        public async Task NegativeOverflow()
        {
            var expected = new decimal?[] { -decimal.MaxValue, -decimal.MaxValue };
            var source = new RealAsyncEnumerable<decimal?>(expected);

            Assert.AreEqual(expected.Sum(), await source.SumAsync());
        }

        [TestMethod]
        [TestCategory(CategoryLinqSumAsync)]
        public async Task NaN()
        {
            var expected = new double[] { 1, 2, 3, 1.0/0.0 };
            var source = expected.AsAsyncEnumerable();

            Assert.AreEqual(expected.Sum(), await source.SumAsync());
        }

        [TestMethod]
        [TestCategory(CategoryLinqSumAsync)]
        public async Task SimpleWithProjection()
        {
            var expected = new[] { "x", "xy", "xyz" };
            var source = new RealAsyncEnumerable<string>(expected);
            var projection = new Func<string, int>(x => x.Length);

            Assert.AreEqual(expected.Sum(projection), await source.SumAsync(projection));
        }
    }
}
