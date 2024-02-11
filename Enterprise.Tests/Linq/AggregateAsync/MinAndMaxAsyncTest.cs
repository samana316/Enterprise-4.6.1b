using System;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Tests.Linq.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Enterprise.Core.Linq.AsyncEnumerable;

namespace Enterprise.Tests.Linq.MinMaxAsync
{
    [TestClass]
    public sealed class AnyAndMaxAsyncTest
    {
        private const string CategoryLinqMinAsync = "Linq.MinAsync";

        private const string CategoryLinqMaxAsync = "Linq.MaxAsync";

        [TestMethod]
        [TestCategory(CategoryLinqMaxAsync)]
        [TestCategory(CategoryLinqMinAsync)]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ArgumentValidation()
        {
            var source = default(IAsyncEnumerable<decimal>);
            await Task.WhenAll(source.MaxAsync(), source.MinAsync());
        }

        [TestMethod]
        [TestCategory(CategoryLinqMaxAsync)]
        [TestCategory(CategoryLinqMinAsync)]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task EmptySequence()
        {
            var empty = Empty<int>();
            var nullableEmpty = Empty<int?>();

            Assert.IsNull(await nullableEmpty.MaxAsync());
            Assert.IsNull(await nullableEmpty.MinAsync());

            await Task.WhenAll(empty.MaxAsync(), empty.MinAsync());
        }

        [TestMethod]
        [TestCategory(CategoryLinqMaxAsync)]
        [TestCategory(CategoryLinqMinAsync)]
        public async Task SequenceOfNull()
        {
            var source = new RealAsyncEnumerable<long?>(null, null, null);
            Assert.IsNull(await source.MaxAsync());
            Assert.IsNull(await source.MinAsync());
        }

        [TestMethod]
        [TestCategory(CategoryLinqMaxAsync)]
        [TestCategory(CategoryLinqMinAsync)]
        public async Task Primitive()
        {
            var source = new RealAsyncEnumerable<float>(1, 2, 3, 0, 0, 3, 2, 1);
            Assert.AreEqual(3, await source.MaxAsync());
            Assert.AreEqual(0, await source.MinAsync());
        }

        [TestMethod]
        [TestCategory(CategoryLinqMaxAsync)]
        [TestCategory(CategoryLinqMinAsync)]
        public async Task PrimitiveNullable()
        {
            var source = new RealAsyncEnumerable<double?>(1, 2, 3, null, 0, null, 0, 3, 2, 1);
            Assert.AreEqual(3, await source.MaxAsync());
            Assert.AreEqual(0, await source.MinAsync());
        }

        [TestMethod]
        [TestCategory(CategoryLinqMaxAsync)]
        [TestCategory(CategoryLinqMinAsync)]
        public async Task PrimitiveProjection()
        {
            var source = new RealAsyncEnumerable<string>("", "Address", "Contract", "Binding");
            var projection = new Func<string, int>(x => x.Length);
            Assert.AreEqual(8, await source.MaxAsync(projection));
            Assert.AreEqual(0, await source.MinAsync(projection));
        }

        [TestMethod]
        [TestCategory(CategoryLinqMaxAsync)]
        [TestCategory(CategoryLinqMinAsync)]
        public async Task PrimitiveProjectionNullable()
        {
            var source = new RealAsyncEnumerable<string>("", "Address", "Contract", "Binding");
            var projection = new Func<string, int?>(x => x.Length <= 0 ? default(int?) : 1);
            Assert.AreEqual(1, await source.MaxAsync(projection));
            Assert.AreEqual(1, await source.MinAsync(projection));
        }

        [TestMethod]
        [TestCategory(CategoryLinqMaxAsync)]
        [TestCategory(CategoryLinqMinAsync)]
        public async Task Generic()
        {
            var date1 = DateTime.Today;
            var date2 = DateTime.Now;
            var source = new RealAsyncEnumerable<DateTime>(date1, date2);
            Assert.AreEqual(date2, await source.MaxAsync());
            Assert.AreEqual(date1, await source.MinAsync());
        }

        [TestMethod]
        [TestCategory(CategoryLinqMaxAsync)]
        [TestCategory(CategoryLinqMinAsync)]
        public async Task GenericNullable()
        {
            var source = new RealAsyncEnumerable<string>("", "Address", "Contract", "Binding");
            Assert.AreEqual("Contract", await source.MaxAsync());
            Assert.AreEqual("", await source.MinAsync());
        }

        [TestMethod]
        [TestCategory(CategoryLinqMaxAsync)]
        [ExpectedException(typeof(ArgumentException))]
        public async Task IncomparableValues()
        {
            var source = Create<Func<string, bool>>(async (yield, cancellationToken) => 
            {
                await yield.ReturnAsync(string.IsNullOrEmpty, cancellationToken);
                await yield.ReturnAsync(string.IsNullOrWhiteSpace, cancellationToken);
            });

            await Task.WhenAll(source.MaxAsync(), source.MinAsync());
        }
    }
}
