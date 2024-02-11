using System;
using System.Linq;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Tests.Linq.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enterprise.Tests.Linq.Cast
{
    [TestClass]
    public sealed class CastTest
    {
        private const string CategoryLinqCast = "Linq.Cast";

        [TestMethod]
        [TestCategory(CategoryLinqCast)]
        public void OriginalSourceReturnedDueToGenericCovariance()
        {
            IAsyncEnumerable strings = new ThrowAsyncEnumerable<string>();
            Assert.AreSame(strings, strings.Cast<object>());
        }

        [TestMethod]
        [TestCategory(CategoryLinqCast)]
        public async Task CastSimple()
        {
            var expected = new object[] { "first", "second", "third" };
            IAsyncEnumerable strings = expected.AsAsyncEnumerable();
            var query = strings.Cast<string>();

            Assert.IsTrue(await query.SequenceEqualAsync(expected));
        }

        [TestMethod]
        [TestCategory(CategoryLinqCast)]
        public async Task CastWithFrom()
        {
            var expected = new object[] { "first", "second", "third" };
            IAsyncEnumerable strings = expected.AsAsyncEnumerable();
            var query = from string x in strings
                        select x;

            Assert.IsTrue(await query.SequenceEqualAsync(expected));
        }

        [TestMethod]
        [TestCategory(CategoryLinqCast)]
        public async Task CastWithJoin()
        {
            var ints = AsyncEnumerable.Range(0, 10);
            IAsyncEnumerable strings = new RealAsyncEnumerable<string>("first", "second", "third");

            var query = from x in ints
                        join string y in strings on x equals y.Length
                        select x + ":" + y;

            var expected = new [] { "5:first", "5:third", "6:second" };
            Assert.IsTrue(await query.SequenceEqualAsync(expected));
        }

        [TestMethod]
        [TestCategory(CategoryLinqCast)]
        [ExpectedException(typeof(InvalidCastException))]
        public async Task InvalidCast()
        {
            var source = new RealAsyncEnumerable<int>(1);
            var query = source.Cast<DateTime>();

            using (var iterator = query.GetAsyncEnumerator())
            {
                await iterator.MoveNextAsync();
            }
        }

        [TestMethod]
        [TestCategory(CategoryLinqCast)]
        public async Task OfTypeSimple()
        {
            var source = AsyncEnumerable.Create<object>(async (yield, cancellationToken) => 
            {
                await yield.ReturnAsync(1, cancellationToken);
                await yield.ReturnAsync(2.0, cancellationToken);
                await yield.ReturnAsync("Three", cancellationToken);
                await yield.ReturnAsync(new { x = 1 }, cancellationToken);
            });

            var query1 = source.OfType<int>();
            Assert.IsTrue(await query1.SequenceEqualAsync(new[] { 1 }));

            var query2 = source.OfType<IConvertible>();
            Assert.IsTrue(await query2.SequenceEqualAsync(new IConvertible[] { 1, 2.0, "Three" }));
        }
    }
}