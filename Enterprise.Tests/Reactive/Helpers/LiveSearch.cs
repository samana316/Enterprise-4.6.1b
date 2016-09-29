using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Core.Reactive;
using Enterprise.Core.Reactive.Linq;

namespace Enterprise.Tests.Reactive.Helpers
{
    internal static class LiveSearch
    {
        public static IAsyncObservable<IAsyncObservable<TSource>> CreateMock<TSource>(
            TimeSpan delay)
        {
            IAsyncYieldBuilder<IAsyncObservable<TSource>> producer = new MockImpl<TSource>(delay);

            return AsyncObservable.Create<IAsyncObservable<TSource>>(producer.RunAsync);
        }

        private class MockImpl<TSource> : IAsyncYieldBuilder<IAsyncObservable<TSource>>
        {
            private TimeSpan delay;

            public MockImpl(
                TimeSpan delay)
            {
                this.delay = delay;
            }

            public async Task RunAsync(
                IAsyncYield<IAsyncObservable<TSource>> yield, 
                CancellationToken cancellationToken)
            {
                var s1 = FromMarbleDiagram.Create<TSource>("  -1--1--1--1|", delay);
                var s2 = FromMarbleDiagram.Create<TSource>("      --2-2--2--2|", delay);
                var s3 = FromMarbleDiagram.Create<TSource>("          -3--3|", delay);

                await yield.ReturnAsync(s1, cancellationToken);
                await yield.ReturnAsync(s2, cancellationToken);
                await yield.ReturnAsync(s3, cancellationToken);
            }
        }
    }
}
