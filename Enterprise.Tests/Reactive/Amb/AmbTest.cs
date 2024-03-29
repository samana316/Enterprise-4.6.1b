﻿using System;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Core.Reactive.Linq;
using Enterprise.Core.Reactive.Subjects;
using Enterprise.Tests.Reactive.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Enterprise.Core.Reactive.Linq.AsyncObservable;

namespace Enterprise.Tests.Reactive.Amb
{
    [TestClass]
    public class AmbTest
    {
        private const int DefaultTimeout = 1000;

        private const string CategoryReactiveAmb = "Reactive.Amb";

        [TestMethod]
        [TestCategory(CategoryReactiveAmb)]
        public async Task Simple()
        {
            var expected = AsyncEnumerable.Range(1, 3);
            var first = expected.ToAsyncObservable();
            var second = AsyncObservable.Range(4, 3).Delay(TimeSpan.FromMilliseconds(100));

            var query1 = first.Amb(second);
            Assert.IsTrue(await query1.SequenceEqual(expected));

            var query2 = second.Amb(first);
            Assert.IsTrue(await query2.SequenceEqual(expected));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveAmb)]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Empty()
        {
            var expected = AsyncEnumerable.Range(1, 3);
            var first = expected.ToAsyncObservable().Delay(TimeSpan.FromMilliseconds(10));
            var second = Empty<int>();

            await first.Amb(second);
        }

        [TestMethod]
        [TestCategory(CategoryReactiveAmb)]
        [Timeout(DefaultTimeout)]
        public async Task Never()
        {
            var expected = AsyncEnumerable.Range(1, 3);
            var first = expected.ToAsyncObservable().Delay(TimeSpan.FromMilliseconds(10));
            var second = Never<int>();

            var query = first.Amb(second);
            Assert.IsTrue(await query.SequenceEqual(expected));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveAmb)]
        public async Task Enumerables()
        {
            var expected = AsyncEnumerable.Range(1, 3);
            var first = expected.ToAsyncObservable();
            var second = AsyncObservable.Range(4, 3).Delay(TimeSpan.FromMilliseconds(100));
            var third = Empty<int>().Delay(TimeSpan.FromSeconds(1));

            var query = Amb<int>(first, second, third);
            Assert.IsTrue(await query.SequenceEqual(expected));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveAmb)]
        public async Task Subjects()
        {
            var observer = new SpyAsyncObserver<int>();
            var s1 = new ReplayAsyncSubject<int>();
            var s2 = new ReplayAsyncSubject<int>();
            var s3 = new ReplayAsyncSubject<int>();
            var result = AsyncObservable.Amb(s1, s2, s3);

            await s1.OnNextAsync(1);
            await s2.OnNextAsync(2);
            await s3.OnNextAsync(3);
            await s1.OnNextAsync(1);
            await s2.OnNextAsync(2);
            await s3.OnNextAsync(3);
            s1.OnCompleted();
            s2.OnCompleted();
            s3.OnCompleted();

            await result.SubscribeAsync(observer);

            Assert.IsTrue(await observer.Items.SequenceEqualAsync(new[] { 1, 1 }));
        }
    }
}
