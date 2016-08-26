using System;
using System.Linq;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Tests.Linq.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Enterprise.Core.Linq.AsyncEnumerable;

namespace Enterprise.Tests.Linq.SetOperations
{
    [TestClass]
    public sealed class ExceptTest
    {
        private const string CategoryLinqExcept = "Linq.Except";

        [TestMethod]
        [TestCategory(CategoryLinqExcept)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateNullFirst()
        {
            IAsyncEnumerable<int> first = null;
            var query = first.Except(null);
        }

        [TestMethod]
        [TestCategory(CategoryLinqExcept)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateNullSecond()
        {
            var first = Empty<int>();
            var query = first.Except(null);
        }

        [TestMethod]
        [TestCategory(CategoryLinqExcept)]
        public void NoSequencesUsedBeforeIteration()
        {
            var first = new ThrowAsyncEnumerable<int>();
            var second = new ThrowAsyncEnumerable<int>();
            // No exceptions!
            var query = first.Union(second);
            // Still no exceptions… we’re not calling MoveNext.
            using (var iterator = query.GetAsyncEnumerator())
            {
            }
        }

        [TestMethod]
        [TestCategory(CategoryLinqExcept)]
        [ExpectedException(typeof(DivideByZeroException))]
        public async Task SecondSequenceReadFullyOnFirstResultIteration()
        {
            var first = new RealAsyncEnumerable<int>(1);
            var secondQuery = new RealAsyncEnumerable<int>(10, 2, 0).Select(x => 10 / x);

            var query = first.Except(secondQuery);
            using (var iterator = query.GetAsyncEnumerator())
            {
                await iterator.MoveNextAsync();
            }
        }

        [TestMethod]
        [TestCategory(CategoryLinqExcept)]
        [ExpectedException(typeof(DivideByZeroException))]
        public async Task FirstSequenceOnlyReadAsResultsAreRead()
        {
            var firstQuery = new RealAsyncEnumerable<int>(10, 2, 0).Select(x => 10 / x);
            var second = new RealAsyncEnumerable<int>(1);

            var query = firstQuery.Except(second);
            using (var iterator = query.GetEnumerator())
            {
                // We can get the first value with no problems
                Assert.IsTrue(await iterator.MoveNextAsync());
                Assert.AreEqual(5, iterator.Current);

                // Getting at the *second* value of the result sequence requires
                // reading from the first input sequence until the "bad" division
                await iterator.MoveNextAsync();
            }
        }
    }
}
