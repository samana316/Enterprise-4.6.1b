using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Core.Reactive.Linq;
using Enterprise.Core.Reactive.Subjects;
using Enterprise.Tests.Reactive.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enterprise.Tests.Reactive.Subjects
{
    [TestClass]
    public sealed class SubjectTest
    {
        private const int DefaultTimeout = 1000;

        private const string CategoryReactiveSubjects = "Reactive.Subjects";

        [TestMethod]
        [TestCategory(CategoryReactiveSubjects)]
        [Timeout(DefaultTimeout)]
        public async Task AsyncSubjectSimple()
        {
            var observer1 = new SpyAsyncObserver<int>();
            var observer2 = new SpyAsyncObserver<int>();

            var subject = new AsyncSubject<int>();
            var subscription1 = subject.SubscribeAsync(observer1);

            await subject.OnNextAsync(1);
            await subject.OnNextAsync(2);

            var subscription2 = subject.SubscribeAsync(observer2);

            await subject.OnNextAsync(3);

            subscription1.Dispose();

            await subject.OnNextAsync(4);
            subject.OnCompleted();
            await subject.OnNextAsync(5);

            Assert.IsTrue(await observer1.Items.SequenceEqualAsync(new[] { 1, 2, 3 }));
            Assert.IsTrue(await observer2.Items.SequenceEqualAsync(new[] { 3, 4 }));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveSubjects)]
        [Timeout(DefaultTimeout)]
        public async Task ReplayAsyncSubjectSimple()
        {
            var observer1 = new SpyAsyncObserver<int>();
            var observer2 = new SpyAsyncObserver<int>();

            var subject = new ReplayAsyncSubject<int>();
            var subscription1 = subject.SubscribeAsync(observer1);
            await subscription1;

            await subject.OnNextAsync(1);
            await subject.OnNextAsync(2);

            var subscription2 = subject.SubscribeAsync(observer2);
            await subscription2;

            await subject.OnNextAsync(3);
            subscription1.Dispose();

            await subject.OnNextAsync(4);
            subject.OnCompleted();
            await subject.OnNextAsync(5);
            subscription2.Dispose();

            Assert.IsTrue(await observer1.Items.SequenceEqualAsync(new[] { 1, 2, 3 }));
            Assert.IsTrue(await observer2.Items.SequenceEqualAsync(new[] { 1, 2, 3, 4 }));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveSubjects)]
        [Timeout(DefaultTimeout)]
        public async Task ReplayAsyncSubjectByBufferSize()
        {
            var observer = new SpyAsyncObserver<int>();
            var bufferSize = 2;

            var subject = new ReplayAsyncSubject<int>(bufferSize);
            await subject.OnNextAsync(1);
            await subject.OnNextAsync(2);
            await subject.OnNextAsync(3);
            await subject.SubscribeAsync(observer);
            await subject.OnNextAsync(4);

            Assert.IsTrue(await observer.Items.SequenceEqualAsync(new[] { 2, 3, 4 }));
        }

        [TestMethod]
        [TestCategory(CategoryReactiveSubjects)]
        [Timeout(DefaultTimeout)]
        public async Task ReplayAsyncSubjectQuery()
        {
            var observer = new SpyAsyncObserver<int>();
            var subject = new ReplayAsyncSubject<int>();
            await subject.OnNextAsync(1);
            await subject.OnNextAsync(2);
            await subject.OnNextAsync(3);
            await subject.OnNextAsync(4);

            var query =
                from item in subject
                where item > 1
                select item;

            await query.SubscribeAsync(observer);

            Assert.IsTrue(await observer.Items.SequenceEqualAsync(new[] { 2, 3, 4 }));
        }
    }
}
