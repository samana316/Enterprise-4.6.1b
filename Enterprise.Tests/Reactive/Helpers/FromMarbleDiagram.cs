using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Core.Reactive;
using Enterprise.Core.Reactive.Linq;

namespace Enterprise.Tests.Reactive.Helpers
{
    internal static class FromMarbleDiagram
    {
        public static IAsyncObservable<TSource> Create<TSource>(
            string marbleDiagram,
            TimeSpan delay)
        {
            var producer = new FromMarbleDiagramImpl<TSource>(marbleDiagram, delay);

            return AsyncObservable.Create<TSource>(producer.RunAsync);
        }

        private sealed class FromMarbleDiagramImpl<TSource> : IAsyncYieldBuilder<TSource>
        {
            private const char terminating = '|';

            private static readonly IEnumerable<char> skips = " -";

            private readonly string marbleDiagram;

            private readonly TimeSpan delay;

            public FromMarbleDiagramImpl(
                string marbleDiagram, 
                TimeSpan delay)
            {
                this.marbleDiagram = marbleDiagram;
                this.delay = delay;
            }

            public Task RunAsync(
                IAsyncYield<TSource> yield, 
                CancellationToken cancellationToken)
            {
                var source = this.marbleDiagram.ToAsyncObservable();

                return source.ForEachAsync(async (value, cancellationToken2) => 
                {
                    if (value == terminating)
                    {
                        yield.Break();
                    }

                    await Task.Delay(this.delay, cancellationToken2);

                    if (!skips.Contains(value))
                    {
                        var casted = (TSource)Convert.ChangeType(value, typeof(TSource));
                        await yield.ReturnAsync(casted, cancellationToken2);
                    }
                }, cancellationToken);
            }
        }
    }
}
