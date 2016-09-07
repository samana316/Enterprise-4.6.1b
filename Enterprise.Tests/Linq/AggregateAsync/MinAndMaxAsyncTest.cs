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
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ArgumentValidation()
        {
            var source = default(IAsyncEnumerable<decimal>);
            await source.MaxAsync();
        }

        [TestMethod]
        [TestCategory(CategoryLinqMaxAsync)]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task EmptySequence()
        {
            var empty = Empty<int>();
            var nullableEmpty = Empty<int?>();

            Assert.IsNull(await nullableEmpty.MaxAsync());
            await empty.MaxAsync();
        }

        [TestMethod]
        [TestCategory(CategoryLinqMaxAsync)]
        public async Task SequenceOfNull()
        {
            var source = new RealAsyncEnumerable<long?>(null, null, null);
            Assert.IsNull(await source.MaxAsync());
        }

        [TestMethod]
        [TestCategory(CategoryLinqMaxAsync)]
        public async Task Primitive()
        {
            var source = new RealAsyncEnumerable<float>(1, 2, 3, 0, 0, 3, 2, 1);
            Assert.AreEqual(3, await source.MaxAsync());
        }

        [TestMethod]
        [TestCategory(CategoryLinqMaxAsync)]
        public async Task PrimitiveNullable()
        {
            var source = new RealAsyncEnumerable<double?>(1, 2, 3, null, 0, null, 0, 3, 2, 1);
            Assert.AreEqual(3, await source.MaxAsync());
        }

        [TestMethod]
        [TestCategory(CategoryLinqMaxAsync)]
        public async Task PrimitiveProjection()
        {
            var source = new RealAsyncEnumerable<string>("", "Address", "Contract", "Binding");
            Assert.AreEqual(8, await source.MaxAsync(x => x.Length));
        }

        [TestMethod]
        [TestCategory(CategoryLinqMaxAsync)]
        public async Task PrimitiveProjectionNullable()
        {
            var source = new RealAsyncEnumerable<string>("", "Address", "Contract", "Binding");
            Assert.AreEqual(1, await source.MaxAsync(x => x.Length <= 0 ? default(int?) : 1));
        }

        [TestMethod]
        [TestCategory(CategoryLinqMaxAsync)]
        public async Task Generic()
        {
            var date1 = DateTime.Today;
            var date2 = DateTime.Now;
            var source = new RealAsyncEnumerable<DateTime>(date2);
            Assert.AreEqual(date2, await source.MaxAsync());
        }

        [TestMethod]
        [TestCategory(CategoryLinqMaxAsync)]
        public async Task GenericNullable()
        {
            var source = new RealAsyncEnumerable<string>("", "Address", "Contract", "Binding");
            Assert.AreEqual("Contract", await source.MaxAsync());
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

            var max = await source.MaxAsync();
        }
    }
}
