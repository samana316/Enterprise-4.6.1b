using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Tests.Linq.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enterprise.Tests.Linq.SetOperations
{
    [TestClass]
    public sealed class DistinctTest
    {
        private const string CategoryLinqDistinct = "Linq.Distinct";

        [TestMethod]
        [TestCategory(CategoryLinqDistinct)]
        public async Task DefaultStringComparer()
        {
            var source = new RealAsyncEnumerable<string>("ABC", "abc", "xyz");
            var query = source.Distinct();

            Assert.IsTrue(await query.SequenceEqualAsync(source));
        }

        [TestMethod]
        [TestCategory(CategoryLinqDistinct)]
        public async Task CaseInsensitiveOrdinalComparer()
        {
            var source = new RealAsyncEnumerable<string>("ABC", "abc", "xyz");
            var query = source.Distinct(StringComparer.OrdinalIgnoreCase);

            Assert.IsTrue(await query.SequenceEqualAsync(new[] { "ABC", "xyz" }));
        }

        [TestMethod]
        [TestCategory(CategoryLinqDistinct)]
        public async Task ObjectIdentityComparer()
        {
            var source = AsyncEnumerable.Create<string>(async (yield, cancellationToken) => 
            {
                await yield.ReturnAsync("ABC", cancellationToken);
                await yield.ReturnAsync("abc".ToUpper(), cancellationToken);
            });

            var query = source.Distinct(TestEqualityComparer<string>.Instance);

            Assert.IsTrue(await query.SequenceEqualAsync(new[] { "ABC", "ABC" }));
        }

        [TestMethod]
        [TestCategory(CategoryLinqDistinct)]
        public async Task SingleNull()
        {
            var source = new RealAsyncEnumerable<string>("ABC", "abc", "xyz", null);
            var query = source.Distinct();

            Assert.IsTrue(await query.SequenceEqualAsync(source));
        }

        [TestMethod]
        [TestCategory(CategoryLinqDistinct)]
        public async Task DuplicateNull()
        {
            var source = new RealAsyncEnumerable<string>("ABC", "abc", null, "xyz", null, null);
            var query = source.Distinct();

            Assert.IsTrue(await query.SequenceEqualAsync(new[] { "ABC", "abc", null, "xyz" }));
        }

        [TestMethod]
        [TestCategory(CategoryLinqDistinct)]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ThrowOnDuplicateNull()
        {
            var source = new RealAsyncEnumerable<string>("ABC", "abc", "xyz", null, null);
            var query = source.Distinct(TestEqualityComparer<string>.Instance);

            Assert.IsTrue(await query.SequenceEqualAsync(source));
        }

        private sealed class TestEqualityComparer<T> : IEqualityComparer<T>
        {
            public static IEqualityComparer<T> Instance = new TestEqualityComparer<T>();

            private TestEqualityComparer()
            {
            }

            public bool Equals(
                T x, 
                T y)
            {
                if (ReferenceEquals(null, x))
                {
                    throw new ArgumentNullException(nameof(x));
                }

                if (ReferenceEquals(null, y))
                {
                    throw new ArgumentNullException(nameof(y));
                }

                return ReferenceEquals(x, y);
            }

            public int GetHashCode(
                T obj)
            {
                if (ReferenceEquals(obj, null))
                {
                    throw new ArgumentNullException(nameof(obj));
                }

                return EqualityComparer<T>.Default.GetHashCode(obj);
            }
        }
    }
}
