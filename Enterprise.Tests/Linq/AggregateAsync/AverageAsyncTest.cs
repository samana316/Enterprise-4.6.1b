using System;
using System.Linq;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Tests.Linq.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Enterprise.Core.Linq.AsyncEnumerable;

namespace Enterprise.Tests.Linq.AverageAsync
{
    [TestClass]
    public class AverageAsyncTest
    {
        private const string CategoryLinqAverageAsync = "Linq.AverageAsync";

        [TestMethod]
        [TestCategory(CategoryLinqAverageAsync)]
        public async Task Simple()
        {
            var expected = new[] { 1, 2, 3 };
            var source = new RealAsyncEnumerable<int>(expected);

            Assert.AreEqual(expected.Average(), await source.AverageAsync());
        }

        [TestMethod]
        [TestCategory(CategoryLinqAverageAsync)]
        public async Task Empty()
        {
            var source = Empty<long?>();

            Assert.AreEqual(source.Average(), await source.AverageAsync());
        }

        [TestMethod]
        [TestCategory(CategoryLinqAverageAsync)]
        public async Task SimpleWithOnlyNull()
        {
            var expected = new float?[] { null, null, null, null };
            var source = new RealAsyncEnumerable<float?>(expected);

            Assert.AreEqual(expected.Average(), await source.AverageAsync());
        }

        [TestMethod]
        [TestCategory(CategoryLinqAverageAsync)]
        public async Task SimpleWithSomeNull()
        {
            var expected = new double?[] { 1.1, 2.2, null, 3.3, null };
            var source = new RealAsyncEnumerable<double?>(expected);

            Assert.AreEqual(expected.Average(), await source.AverageAsync());
        }

        [TestMethod]
        [TestCategory(CategoryLinqAverageAsync)]
        public async Task NaN()
        {
            var expected = new double[] { 1, 2, 3, 1.0 / 0.0 };
            var source = expected.AsAsyncEnumerable();

            Assert.AreEqual(expected.Average(), await source.AverageAsync());
        }

        [TestMethod]
        [TestCategory(CategoryLinqAverageAsync)]
        public async Task SimpleWithProjection()
        {
            var expected = new[] { "x", "xy", "xyz" };
            var source = new RealAsyncEnumerable<string>(expected);
            var projection = new Func<string, int>(x => x.Length);

            Assert.AreEqual(expected.Average(projection), await source.AverageAsync(projection));
        }

        [TestMethod]
        [TestCategory(CategoryLinqAverageAsync)]
        public async Task OverflowInt32()
        {
            var source = new RealAsyncEnumerable<int>(
                int.MaxValue, int.MaxValue, -int.MaxValue, -int.MaxValue);

            Assert.AreEqual(0, await source.AverageAsync());
        }

        [TestMethod]
        [TestCategory(CategoryLinqAverageAsync)]
        [ExpectedException(typeof(OverflowException))]
        public async Task OverflowInt64()
        {
            var source = new RealAsyncEnumerable<long>(
                long.MaxValue, long.MaxValue, -long.MaxValue, -long.MaxValue);

            await source.AverageAsync();
        }

        [TestMethod]
        [TestCategory(CategoryLinqAverageAsync)]
        public async Task OverflowSingle()
        {
            var source = new RealAsyncEnumerable<float>(
                float.MaxValue, float.MaxValue, -float.MaxValue, -float.MaxValue);

            Assert.AreEqual(0, await source.AverageAsync());
        }

        [TestMethod]
        [TestCategory(CategoryLinqAverageAsync)]
        public async Task OverflowDouble()
        {
            var source = new RealAsyncEnumerable<double>(
                double.MaxValue, double.MaxValue, -double.MaxValue, -double.MaxValue);

            Assert.AreEqual(double.PositiveInfinity, await source.AverageAsync());
        }

        [TestMethod]
        [TestCategory(CategoryLinqAverageAsync)]
        [ExpectedException(typeof(OverflowException))]
        public async Task OverflowDecimal()
        {
            var source = new RealAsyncEnumerable<decimal>(
                decimal.MaxValue, decimal.MaxValue, -decimal.MaxValue, -decimal.MaxValue);

            await source.AverageAsync();
        }
    }
}
