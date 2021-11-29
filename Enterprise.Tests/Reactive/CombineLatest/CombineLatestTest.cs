using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Core.Reactive.Linq;
using Enterprise.Tests.Reactive.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Enterprise.Core.Reactive.Linq.AsyncObservable;

namespace Enterprise.Tests.Reactive.CombineLatest
{
    [TestClass]
    public sealed class CombineLatestTest
    {
        private const int DefaultTimeout = 3000;

        private const string CategoryReactiveCombineLatest = "Reactive.CombineLatest";

        [TestMethod]
        [TestCategory(CategoryReactiveCombineLatest)]
        [Timeout(DefaultTimeout)]
        public async Task Simple3()
        {
            var s1 = AsyncObservable.Range(1, 3).Select(x => x.ToString());
            var s2 = "abc".ToAsyncObservable().Select(x => x.ToString());

            var query = s1.CombineLatest(s2, (x, y) => new { x, y });
            var observer = query.CreateSpyAsyncObserver();
            await query.SubscribeAsync(observer, CancellationToken.None);

            Assert.IsTrue(observer.IsCompleted);
            Assert.IsFalse(observer.Error.InnerExceptions.Any());
            // Race condition.
        }

        [TestMethod]
        [TestCategory(CategoryReactiveCombineLatest)]
        [Timeout(DefaultTimeout)]
        public async Task Throw3()
        {
            var s1 = AsyncObservable.Range(1, 3);
            var s2 = Throw<int>(new InvalidOperationException());

            var query = s1.CombineLatest(s2, (x, y) => new { x, y });
            var observer = query.CreateSpyAsyncObserver();
            await query.SubscribeAsync(observer, CancellationToken.None);

            Assert.IsTrue(observer.IsCompleted);
            Assert.IsTrue(observer.Error.InnerExceptions.Any());
            Assert.IsFalse(await observer.Items.AnyAsync());
        }

        [TestMethod]
        [TestCategory(CategoryReactiveCombineLatest)]
        [Timeout(DefaultTimeout)]
        public async Task Infinite3()
        {
            var delay = TimeSpan.FromMilliseconds(100);
            var s1 = AsyncObservable.Interval(delay);
            var s2 = AsyncObservable.Interval(delay).Select(x => x + 100);

            var query = s1.CombineLatest(s2, (x, y) => new { x, y }).Take(3);
            var observer = query.CreateSpyAsyncObserver();
            await query.SubscribeAsync(observer, CancellationToken.None);

            Assert.IsTrue(observer.IsCompleted);
            Assert.IsFalse(observer.Error.InnerExceptions.Any());
            Assert.AreEqual(3, await observer.Items.CountAsync());
        }

        [TestMethod]
        [TestCategory(CategoryReactiveCombineLatest)]
        [Timeout(DefaultTimeout)]
        public async Task MarbleDiagrams3()
        {
            var delay = TimeSpan.FromMilliseconds(100);
            var s1 = FromMarbleDiagram.Create<string>("---1---2---3---", delay);
            var s2 = FromMarbleDiagram.Create<string>("--a------bc----", delay);

            var query = s1.CombineLatest(s2, (x, y) => new { x, y});
            var observer = query.CreateSpyAsyncObserver();
            await query.SubscribeAsync(observer, CancellationToken.None);

            Assert.IsTrue(observer.IsCompleted);
            Assert.IsFalse(observer.Error.InnerExceptions.Any());
            Assert.IsTrue(await observer.Items.SequenceEqualAsync(new[] 
            {
                new { x = "1", y = "a" },
                new { x = "2", y = "a" },
                new { x = "2", y = "b" },
                new { x = "2", y = "c" },
                new { x = "3", y = "c" }
            }));
        }
    }
}
