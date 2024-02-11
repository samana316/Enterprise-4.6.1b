using System;
using System.Linq;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Core.Reactive.Linq;
using Enterprise.Tests.Reactive.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enterprise.Tests.Reactive.Generate
{
    [TestClass]
    public sealed class GenerateTest
    {
        private const int DefaultTimeout = 1000;

        private const string CategoryReactiveGenerate = "Reactive.Generate";
        
        [TestMethod]
        [TestCategory(CategoryReactiveGenerate)]
        [Timeout(DefaultTimeout)]
        public async Task Simple()
        {
            int start = 1, count = 10, max = start + count;
            var expected = AsyncEnumerable.Range(start, count);
            var source = AsyncObservable.Generate(
                start,
                value => value < max,
                value => value + 1,
                value => value);

            var observer = new SpyAsyncObserver<int>();
            await source.SubscribeAsync(observer);

            Assert.IsTrue(await observer.Items.SequenceEqualAsync(expected));
            Assert.IsTrue(observer.IsCompleted);
            Assert.IsFalse(observer.Error.InnerExceptions.Any());
        }

        [TestMethod]
        [TestCategory(CategoryReactiveGenerate)]
        [Timeout(DefaultTimeout)]
        public async Task Timer()
        {
            var source = AsyncObservable.Generate(
                0L,
                i => i < 1,
                i => i + 1,
                i => i,
                i => TimeSpan.FromMilliseconds(10));

            var observer = new SpyAsyncObserver<long>();
            using (source.SubscribeAsync(observer))
            {
                await Task.Delay(20);
            }

            await Task.Delay(20);

            Assert.AreEqual(0, await observer.Items.SingleAsync());
            Assert.IsTrue(observer.IsCompleted);
            Assert.IsFalse(observer.Error.InnerExceptions.Any());
        }

        [TestMethod]
        [TestCategory(CategoryReactiveGenerate)]
        [Timeout(DefaultTimeout)]
        public async Task Interval()
        {
            var source = AsyncObservable.Generate(
                0L,
                i => true,
                i => i + 1,
                i => i,
                i => TimeSpan.FromMilliseconds(10));

            var observer = new SpyAsyncObserver<long>();
            using (source.SubscribeAsync(observer))
            {
                await Task.Delay(50);

                Assert.IsTrue(await observer.Items.AnyAsync());
                Assert.IsTrue(await observer.Items.CountAsync() < 10);
                Assert.IsFalse(observer.IsCompleted);
                Assert.IsFalse(observer.Error.InnerExceptions.Any());
            }

            await Task.Delay(50);

            Assert.IsTrue(await observer.Items.AnyAsync());
            Assert.IsTrue(await observer.Items.CountAsync() < 10);
            Assert.IsTrue(observer.IsCompleted);
            Assert.IsTrue(observer.Error.InnerExceptions.Any());
        }
    }
}
