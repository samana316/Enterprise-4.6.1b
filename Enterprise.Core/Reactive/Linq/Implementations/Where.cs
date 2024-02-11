using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;

namespace Enterprise.Core.Reactive.Linq.Implementations
{
    internal sealed class Where<TSource> : AsyncObservableBase<TSource>
    {
        private readonly IAsyncObservable<TSource> source;

        private readonly Func<TSource, bool> predicate;

        private readonly Func<TSource, int, bool> predicateI;

        public Where(
            IAsyncObservable<TSource> source,
            Func<TSource, bool> predicate)
        {
            this.source = source;
            this.predicate = predicate;
        }

        public Where(
            IAsyncObservable<TSource> source,
            Func<TSource, int, bool> predicate)
        {
            this.source = source;
            this.predicateI = predicate;
        }

        public override AsyncIterator<TSource> Clone()
        {
            if (this.predicateI == null)
            {
                return new Where<TSource>(this.source, this.predicate);
            }

            return new Where<TSource>(this.source, this.predicateI);
        }

        protected override Task ProduceAsync(
            IAsyncYield<TSource> yield,
            CancellationToken cancellationToken)
        {
            if (this.predicateI == null)
            {
                return this.WhereImplAsync(yield, cancellationToken);
            }

            return this.WhereImplIAsync(yield, cancellationToken);
        }

        private Task WhereImplAsync(
            IAsyncYield<TSource> yield,
            CancellationToken cancellationToken)
        {
            return this.source.ForEachAsync(async (item, cancellationToken2) =>
            {
                if (this.predicate(item))
                {
                    await yield.ReturnAsync(item, cancellationToken2);
                }
            }, cancellationToken);
        }

        private Task WhereImplIAsync(
            IAsyncYield<TSource> yield,
            CancellationToken cancellationToken)
        {
            var index = 0;

            return this.source.ForEachAsync(async (item, cancellationToken2) =>
            {
                if (this.predicateI(item, index))
                {
                    await yield.ReturnAsync(item, cancellationToken2);
                }

                index++;
            }, cancellationToken);
        }
    }
}
