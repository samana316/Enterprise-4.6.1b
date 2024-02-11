using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Core.Reactive.Linq;
using Enterprise.Tests.Reactive.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Enterprise.Core.Reactive.Linq.AsyncObservable;

namespace Enterprise.Tests.Reactive.Switch
{
    [TestClass]
    public sealed class SwitchTest
    {
        private const int DefaultTimeout = 3000;

        private const string CategoryReactiveSwitch = "Reactive.Switch";

        [TestMethod]
        [TestCategory(CategoryReactiveSwitch)]
        [Timeout(DefaultTimeout)]
        public async Task Simple()
        {
            var sources = new[]
            {
                AsyncObservable.Range(1, 2),
                AsyncObservable.Range(11, 2),
                AsyncObservable.Range(21, 2),
                AsyncObservable.Range(31, 2),
            }.ToAsyncObservable();

            var query = sources.Switch();

            var observer = query.CreateSpyAsyncObserver();
            await query.SubscribeAsync(observer, CancellationToken.None);

            Assert.IsTrue(observer.IsCompleted);
            Assert.IsFalse(observer.Error.InnerExceptions.Any());

            var items = await observer.Items.ToListAsync();

            Assert.AreEqual(5, items.Count);
            // Race condition
            CollectionAssert.AreEquivalent(new[] { 1, 11, 21, 31, 32 }, items);
        }

        [TestMethod]
        [TestCategory(CategoryReactiveSwitch)]
        [Timeout(DefaultTimeout)]
        public async Task MarbleDiagram()
        {
            var sources = LiveSearch.CreateMock<string>(TimeSpan.FromMilliseconds(100));
            var query = sources.Switch().Select(x => Convert.ToInt32(x));

            var observer = query.CreateSpyAsyncObserver();
            await query.SubscribeAsync(observer, CancellationToken.None);

            Assert.IsTrue(observer.IsCompleted);
            Assert.IsFalse(observer.Error.InnerExceptions.Any());
            Assert.AreEqual(6, await observer.Items.CountAsync());
            Assert.IsTrue(await observer.Items.SequenceEqualAsync(new[] { 1, 1, 2, 2, 3, 3 }));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveSwitch)]
        [Timeout(DefaultTimeout)]
        public async Task Tasks()
        {
            var sources = new[]
            {
                Task.FromResult(1),
                Task.Run(async () => { await Task.Delay(100); return 2; }),
                Task.Run(async () => { await Task.Delay(500); return 3; }),
                Task.Run(async () => { await Task.Delay(100); return 4; }),
                Task.FromResult(5),
            }.ToAsyncObservable();

            var query = sources.Switch();

            var observer = query.CreateSpyAsyncObserver();
            await query.SubscribeAsync(observer, CancellationToken.None);

            Assert.IsTrue(observer.IsCompleted);
            Assert.IsFalse(observer.Error.InnerExceptions.Any());

            var items = await observer.Items.ToListAsync();

        }
    }
}
