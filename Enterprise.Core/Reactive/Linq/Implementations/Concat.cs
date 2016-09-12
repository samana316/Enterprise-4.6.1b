using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;

namespace Enterprise.Core.Reactive.Linq.Implementations
{
    internal sealed class Concat<TSource> : AsyncObservableBase<TSource>
    {
        private readonly IAsyncObservable<IAsyncObservable<TSource>> sources;

        private readonly IAsyncEnumerable<IAsyncObservable<TSource>> sourcesE;

        public Concat(
            IObservable<IAsyncObservable<TSource>> sources)
        {
            this.sources = sources.AsAsyncObservable();
        }

        public Concat(
            IEnumerable<IAsyncObservable<TSource>> sources)
        {
            this.sourcesE = sources.AsAsyncEnumerable();
        }

        public override AsyncIterator<TSource> Clone()
        {
            if (this.sources == null)
            {
                return new Concat<TSource>(this.sourcesE);
            }

            return new Concat<TSource>(this.sources);
        }

        protected override Task ProduceAsync(
            IAsyncYield<TSource> yield, 
            CancellationToken cancellationToken)
        {
            if (this.sources == null)
            {
                return this.ConcatImplEAsync(yield, cancellationToken);
            }

            return this.ConcatImplAsync(yield, cancellationToken);
        }

        private Task ConcatImplAsync(
            IAsyncYield<TSource> yield,
            CancellationToken cancellationToken)
        {
            return this.sources.ForEachAsync((source, cancellationToken2) => 
            {
                return source.ForEachAsync((item, cancellationToken3) => 
                {
                    return yield.ReturnAsync(item, cancellationToken3);
                }, cancellationToken2);
            }, cancellationToken);
        }

        private Task ConcatImplEAsync(
            IAsyncYield<TSource> yield,
            CancellationToken cancellationToken)
        {
            return this.sourcesE.ForEachAsync((source, cancellationToken2) =>
            {
                return source.ForEachAsync((item, cancellationToken3) =>
                {
                    return yield.ReturnAsync(item, cancellationToken3);
                }, cancellationToken2);
            }, cancellationToken);
        }
    }
}
