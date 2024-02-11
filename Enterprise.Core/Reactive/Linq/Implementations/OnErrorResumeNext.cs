using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;

namespace Enterprise.Core.Reactive.Linq.Implementations
{
    internal sealed class OnErrorResumeNext<TSource> : AsyncObservableBase<TSource>
    {
        private readonly IAsyncEnumerable<IAsyncObservable<TSource>> sources;

        public OnErrorResumeNext(
            IEnumerable<IAsyncObservable<TSource>> sources)
        {
            this.sources = sources.AsAsyncEnumerable();
        }

        public override AsyncIterator<TSource> Clone()
        {
            return new OnErrorResumeNext<TSource>(this.sources);
        }

        protected override Task ProduceAsync(
            IAsyncYield<TSource> yield,
            CancellationToken cancellationToken)
        {
            return this.sources.ForEachAsync(async (source, cancellationToken2) =>
            {
                try
                {
                    await yield.ReturnAllAsync(source, cancellationToken2);
                }
                catch
                {
                }
            }, cancellationToken);
        }
    }
}
