using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Core.Reactive;
using Enterprise.Core.Reactive.Linq;
using Enterprise.Tests.Linq.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Enterprise.Core.Reactive.Linq.AsyncObservable;

namespace Enterprise.Tests.Reactive.Concat
{
    [TestClass]
    public class ConcatTest
    {
        private const int DefaultTimeout = 1000;

        private const string CategoryReactiveConcat = "Reactive.Concat";

        [TestMethod]
        [TestCategory(CategoryReactiveConcat)]
        [ExpectedException(typeof(NotImplementedException))]
        public async Task FirstSequenceIsntAccessedBeforeFirstUse()
        {
            var first = Throw<int>(new NotImplementedException());
            var second = Return(5);

            var query = first.Concat(second);

            using (var iterator = query.GetAsyncEnumerator())
            {
                await iterator.MoveNextAsync();
            }
        }

        [TestMethod]
        [TestCategory(CategoryReactiveConcat)]
        [ExpectedException(typeof(NotImplementedException))]
        public async Task SecondSequenceIsntAccessedBeforeFirstUse()
        {
            var first = Return(5);
            var second = Throw<int>(new NotImplementedException());

            var query = first.Concat(second);

            using (var iterator = query.GetAsyncEnumerator())
            {
                Assert.IsTrue(await iterator.MoveNextAsync());
                Assert.AreEqual(5, iterator.Current);

                await iterator.MoveNextAsync();
            }
        }

        [TestMethod]
        [TestCategory(CategoryReactiveConcat)]
        public async Task Simple()
        {
            var first = AsyncObservable.Range(1, 3);
            var second = AsyncObservable.Range(4, 2);

            var query = first.Concat(second);
            var expected = AsyncEnumerable.Range(1, 5);

            Assert.IsTrue(await query.SequenceEqual(expected));
            Assert.IsTrue(await query.SequenceEqual(expected));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveConcat)]
        public async Task Enumerables()
        {
            var first = Return(0);
            var second = Empty<int>();
            var third = AsyncObservable.Range(1, 3);

            var query = first.Concat(second, third);
            var expected = AsyncEnumerable.Range(0, 3);

            Assert.IsTrue(await query.SequenceEqual(expected));
            Assert.IsTrue(await query.SequenceEqual(expected));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveConcat)]
        public async Task Observables()
        {
            var source = new TestNestedObservable();
            var query = source.Concat();
            var expected = AsyncEnumerable.Range(0, 3);

            Assert.IsTrue(await query.SequenceEqual(expected));
            Assert.IsTrue(await query.SequenceEqual(expected));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveConcat)]
        public async Task Tasks()
        {
            var expected = new[] { "A", "B", "C" };
            var source = expected.ToAsyncObservable().Concat(Return<string>(null));
            Func<string, Task<string>> selectorAsync = async item =>
            {
                await Task.Delay(1);
                await Console.Out.WriteLineAsync("OnNextAsync: " + item);

                return item;
            };

            var query = (
                from item in source
                where item != null && item.Length > 0
                select selectorAsync(item)).Concat();

            Assert.IsTrue(await query.SequenceEqual(expected));
            Assert.IsTrue(await query.SequenceEqual(expected));
        }

        private sealed class TestNestedObservable : IObservable<IAsyncObservable<int>>
        {
            public IDisposable Subscribe(
                IObserver<IAsyncObservable<int>> observer)
            {
                var first = Return(0);
                var second = Empty<int>();
                var third = AsyncObservable.Range(1, 3);

                observer.OnNext(first);
                observer.OnNext(second);
                observer.OnNext(third);

                return null;
            }
        }
    }
}
