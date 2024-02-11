using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;

namespace Enterprise.Core.Reactive.Linq.Implementations
{
    internal sealed class TakeWhile<TSource> : AsyncObservableBase<TSource>
    {
        private readonly IAsyncObservable<TSource> source;

        private readonly Func<TSource, bool> predicate;

        private readonly Func<TSource, int, bool> predicateI;

        public TakeWhile(
            IAsyncObservable<TSource> source,
            Func<TSource, bool> predicate)
        {
            this.source = source;
            this.predicate = predicate;
        }

        public TakeWhile(
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
                return new TakeWhile<TSource>(this.source, this.predicate);
            }

            return new TakeWhile<TSource>(this.source, this.predicateI);
        }

        protected override Task ProduceAsync(
            IAsyncYield<TSource> yield,
            CancellationToken cancellationToken)
        {
            if (this.predicateI == null)
            {
                return this.TakeWhileImplAsync(yield, cancellationToken);
            }

            return this.TakeWhileImplIAsync(yield, cancellationToken);
        }

        private Task TakeWhileImplAsync(
            IAsyncYield<TSource> yield,
            CancellationToken cancellationToken)
        {
            return this.source.ForEachAsync((item, cancellationToken2) =>
            {
                if (!this.predicate(item))
                {
                    yield.Break();
                }

                return yield.ReturnAsync(item, cancellationToken2);
            }, cancellationToken);
        }

        private Task TakeWhileImplIAsync(
            IAsyncYield<TSource> yield,
            CancellationToken cancellationToken)
        {
            var index = 0;

            return this.source.ForEachAsync((item, cancellationToken2) =>
            {
                if (!this.predicateI(item, index))
                {
                    yield.Break();
                }

                index++;
                return yield.ReturnAsync(item, cancellationToken2);
            }, cancellationToken);
        }
    }
}