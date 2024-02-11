using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Tests.Linq.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enterprise.Tests.Linq.SequenceEqualAsync
{
    [TestClass]
    public sealed class SequenceEqualAsyncTest
    {
        private const string CategoryLinqSequenceEqualAsync = "Linq.SequenceEqualAsync";

        [TestMethod]
        [TestCategory(CategoryLinqSequenceEqualAsync)]
        public async Task DifferentLengthsEager()
        {
            var first = new int[1].AsAsyncEnumerable();
            var second = new int[2].AsAsyncEnumerable();

            Assert.IsFalse(await first.SequenceEqualAsync(second));
            Assert.IsFalse(await second.SequenceEqualAsync(first));
        }

        [TestMethod]
        [TestCategory(CategoryLinqSequenceEqualAsync)]
        public async Task DifferentLengthsLazy()
        {
            var first = new RealAsyncEnumerable<int>(1, 2);
            var second = new RealAsyncEnumerable<int>(1, 2, 3);

            Assert.IsFalse(await first.SequenceEqualAsync(second));
            Assert.IsFalse(await second.SequenceEqualAsync(first));
        }

        [TestMethod]
        [TestCategory(CategoryLinqSequenceEqualAsync)]
        public async Task SameLengthWithDifferentComparers()
        {
            var first = new RealAsyncEnumerable<string>("A", "B", "C");
            var second = new RealAsyncEnumerable<string>("a", "b", "c");
            var comparer = StringComparer.OrdinalIgnoreCase;

            Assert.IsTrue(await first.SequenceEqualAsync(second, comparer));
            Assert.IsTrue(await second.SequenceEqualAsync(first, comparer));

            Assert.IsFalse(await first.SequenceEqualAsync(second));
            Assert.IsFalse(await second.SequenceEqualAsync(first));
        }

        [TestMethod]
        [TestCategory(CategoryLinqSequenceEqualAsync)]
        public async Task EqualEager()
        {
            var first = new int[10].AsAsyncEnumerable();
            var second = new int[10].AsAsyncEnumerable();

            Assert.IsTrue(await first.SequenceEqualAsync(second));
            Assert.IsTrue(await second.SequenceEqualAsync(first));
        }

        [TestMethod]
        [TestCategory(CategoryLinqSequenceEqualAsync)]
        public async Task EqualLazy()
        {
            var first = AsyncEnumerable.Range(1, 3);
            var second = new RealAsyncEnumerable<int>(1, 2, 3);

            Assert.IsTrue(await first.SequenceEqualAsync(second));
            Assert.IsTrue(await second.SequenceEqualAsync(first));
        }

        [TestMethod]
        [TestCategory(CategoryLinqSequenceEqualAsync)]
        public async Task NonOptimizationOfUsingSideEffects()
        {
            var random = new Random();
            var foo = AsyncEnumerable.Create<double>(async (yield, cancellationToken) => 
            {
                var size = random.Next(1, 10);

                for (var i = 0; i < size; i++)
                {
                    var next = random.NextDouble();
                    await yield.ReturnAsync(next, cancellationToken);
                }

                yield.Break();
            });

            Assert.IsFalse(await foo.SequenceEqualAsync(foo));
        }

        [TestMethod]
        [TestCategory(CategoryLinqSequenceEqualAsync)]
        public async Task OrderingMatters()
        {
            var first = new RealAsyncEnumerable<int>(1, 2);
            var second = new RealAsyncEnumerable<int>(2, 1);

            Assert.IsFalse(await first.SequenceEqualAsync(second));
            Assert.IsFalse(await second.SequenceEqualAsync(first));
        }

        [TestMethod]
        [TestCategory(CategoryLinqSequenceEqualAsync)]
        [Timeout(3000)]
        public async Task InfiniteSequence()
        {
            var first = new RealAsyncEnumerable<int>(1, 2);
            var second = AsyncEnumerable.Create<int>(async (yield, cancellationToken) =>
            {
                for (var i = 1; true; i++)
                {
                    await yield.ReturnAsync(i, cancellationToken);
                }
            });

            Assert.IsFalse(await first.SequenceEqualAsync(second));
            Assert.IsFalse(await second.SequenceEqualAsync(first));
        }
    }
}
