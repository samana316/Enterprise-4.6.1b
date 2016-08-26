using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Tests.Linq.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Enterprise.Core.Linq.AsyncEnumerable;

namespace Enterprise.Tests.Linq.AggregateAsync
{
    [TestClass]
    public class AggregateAsyncTest
    {
        private const string CategoryLinqAggregateAsync = "Linq.AggregateAsync";

        [TestMethod]
        [TestCategory(CategoryLinqAggregateAsync)]
        public async Task SeededAggregationWithResultSelector()
        {
            var source = new RealAsyncEnumerable<int>( 1, 4, 5 );
            int seed = 5;
            Func<int, int, int> func = (current, value) => current * 2 + value;
            Func<int, string> resultSelector = result => result.ToString(CultureInfo.InvariantCulture);
            // First iteration: 5 * 2 + 1 = 11
            // Second iteration: 11 * 2 + 4 = 26
            // Third iteration: 26 * 2 + 5 = 57
            // Result projection: 57.ToString() = "57"
            Assert.AreEqual("57", await source.AggregateAsync(seed, func, resultSelector));
        }

        [TestMethod]
        [TestCategory(CategoryLinqAggregateAsync)]
        public async Task DifferentSourceAndAccumulatorTypes()
        {
            var largeValue = 2000000000;
            var source = new RealAsyncEnumerable<int>(largeValue, largeValue, largeValue );
            var sum =  await source.AggregateAsync(0L, (acc, value) => acc + value);
            Assert.AreEqual(6000000000L, sum);
            // Just to prove we haven’t missed off a zero…
            Assert.IsTrue(sum > int.MaxValue);
        }

        [TestMethod]
        [TestCategory(CategoryLinqAggregateAsync)]
        public async Task TestFirstOverload()
        {
            Func<int, int, int> accumulator = (a, x) => a + x;

            var source1 = new RealAsyncEnumerable<int>(1, 2, 3);
            var aggregate1 = await source1.AggregateAsync(accumulator);
            Assert.AreEqual(6, aggregate1);

            var source2 = Empty<int>();

            try
            {
                var aggregate2 = await source2.AggregateAsync(accumulator);
                Assert.Fail("This should throw.");
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception);
                Assert.IsTrue(exception is InvalidOperationException);
            }

            var source3 = default(IAsyncEnumerable<int>);
            try
            {
                var aggregate3 = await source3.AggregateAsync(accumulator);
                Assert.Fail("This should throw.");
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception);
                Assert.IsTrue(exception is ArgumentNullException);
            }
        }
    }
}
