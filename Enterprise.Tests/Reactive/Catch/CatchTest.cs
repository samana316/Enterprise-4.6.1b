using System;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Core.Reactive.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Enterprise.Core.Reactive.Linq.AsyncObservable;

namespace Enterprise.Tests.Reactive.Catch
{
    [TestClass]
    public class CatchTest
    {
        private const int DefaultTimeout = 1000;

        private const string CategoryReactiveCatch = "Reactive.Catch";

        [TestMethod]
        [TestCategory(CategoryReactiveCatch)]
        public async Task Simple()
        {
            var first = Throw<int>(new InvalidOperationException());
            var second = Return(1);

            var query = first.Catch(second);

            Assert.AreEqual(1, await query);
        }

        [TestMethod]
        [TestCategory(CategoryReactiveCatch)]
        public async Task Chained()
        {
            var first = Throw<int>(new InvalidOperationException());
            var second = Throw<int>(new InvalidOperationException());
            var third = Return(1);

            var query = Catch<int>(first, second, third);

            Assert.AreEqual(1, await query);
        }

        [TestMethod]
        [TestCategory(CategoryReactiveCatch)]
        public async Task WithHandler()
        {
            var source =
                from item in AsyncObservable.Range(1, 5)
                select item == 5 ? 1 / (5 - item) : item;

            var query = source.Catch<int, DivideByZeroException>(ex => Return(5));

            Assert.IsTrue(await query.SequenceEqual(AsyncEnumerable.Range(1, 5)));
        }
    }
}
