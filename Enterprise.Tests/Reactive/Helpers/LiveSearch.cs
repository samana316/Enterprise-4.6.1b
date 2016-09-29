using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Core.Reactive;
using Enterprise.Core.Reactive.Linq;

namespace Enterprise.Tests.Reactive.Helpers
{
    internal static class LiveSearch
    {
        public static IAsyncObservable<IAsyncObservable<TSource>> Create<TSource>(
            TimeSpan delay,
            params string[] marbleDiagrams)
        {
            IAsyncYieldBuilder<IAsyncObservable<TSource>> producer = new MockImpl<TSource>(delay);

            return AsyncObservable.Create<IAsyncObservable<TSource>>(producer.RunAsync);
        }

        public static IAsyncObservable<IAsyncObservable<TSource>> CreateMock<TSource>(
            TimeSpan delay)
        {
            IAsyncYieldBuilder<IAsyncObservable<TSource>> producer = new MockImpl<TSource>(delay);

            return AsyncObservable.Create<IAsyncObservable<TSource>>(producer.RunAsync);
        }

        private class Impl<TSource> : IAsyncYieldBuilder<IAsyncObservable<TSource>>
        {
            private readonly TimeSpan delay;

            private readonly IEnumerable<string> marbleDiagrams;

            public Impl(
                TimeSpan delay, 
                IEnumerable<string> marbleDiagrams)
            {
                this.delay = delay;
                this.marbleDiagrams = marbleDiagrams;
            }

            public async Task RunAsync(
                IAsyncYield<IAsyncObservable<TSource>> yield, 
                CancellationToken cancellationToken)
            {
                foreach (var marbleDiagram in this.marbleDiagrams)
                {
                    var source = FromMarbleDiagram.Create<TSource>(marbleDiagram, this.delay);

                    await yield.ReturnAsync(source, cancellationToken);
                }
            }
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
                var s1 = FromMarbleDiagram.Create<TSource>("  -1--1--1--1|", this.delay);
                var s2 = FromMarbleDiagram.Create<TSource>("      --2-2--2--2|", this.delay);
                var s3 = FromMarbleDiagram.Create<TSource>("          -3--3|", this.delay);

                await yield.ReturnAsync(s1, cancellationToken);
                await yield.ReturnAsync(s2, cancellationToken);
                await yield.ReturnAsync(s3, cancellationToken);
            }
        }
    }
}
