using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Enterprise.Core.Linq.AsyncEnumerable;

namespace Enterprise.Tests.Linq.Empty
{
    [TestClass]
    public class EmptyTest
    {
        private const int DefaultTimeout = 1000;
        private const string CategoryLinqEmpty = "Linq.Empty";

        [TestMethod]
        [TestCategory(CategoryLinqEmpty)]
        [Timeout(DefaultTimeout)]
        public async Task EmptyContainsNoElements()
        {
            using (var empty = Empty<int>().GetAsyncEnumerator())
            {
                Assert.IsFalse(await empty.MoveNextAsync());
            }
        }

        [TestMethod]
        [TestCategory(CategoryLinqEmpty)]
        [Timeout(DefaultTimeout)]
        public void EmptyIsASingletonPerElementType()
        {
            Assert.AreSame(Empty<int>(), Empty<int>());
            Assert.AreSame(Empty<long>(), Empty<long>());
            Assert.AreSame(Empty<string>(), Empty<string>());
            Assert.AreSame(Empty<object>(), Empty<object>());

            Assert.AreNotSame(Empty<long>(), Empty<int>());
            Assert.AreNotSame(Empty<string>(), Empty<object>());
        }
    }
}
