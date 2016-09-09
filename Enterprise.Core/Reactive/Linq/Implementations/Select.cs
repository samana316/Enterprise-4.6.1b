using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;

namespace Enterprise.Core.Reactive.Linq.Implementations
{
    internal sealed class Select<TSource, TResult> : AsyncObservableBase<TResult>
    {
        private readonly IAsyncObservable<TSource> source;

        private readonly Func<TSource, TResult> selector;

        private readonly Func<TSource, int, TResult> selectorI;

        public Select(
            IAsyncObservable<TSource> source,
            Func<TSource, TResult> selector)
        {
            this.source = source;
            this.selector = selector;
        }

        public Select(
            IAsyncObservable<TSource> source,
            Func<TSource, int, TResult> selector)
        {
            this.source = source;
            this.selectorI = selector;
        }

        public override AsyncIterator<TResult> Clone()
        {
            if (this.selectorI == null)
            {
                return new Select<TSource, TResult>(this.source, this.selector);
            }

            return new Select<TSource, TResult>(this.source, this.selectorI);
        }

        protected override Task ProduceAsync(
            IAsyncYield<TResult> yield,
            CancellationToken cancellationToken)
        {
            if (this.selectorI == null)
            {
                return this.SelectImplAsync(yield, cancellationToken);
            }

            return this.SelectImplIAsync(yield, cancellationToken);
        }

        private Task SelectImplAsync(
            IAsyncYield<TResult> yield,
            CancellationToken cancellationToken)
        {
            return this.source.ForEachAsync(async (item, cancellationToken2) =>
            {
                var result = this.selector(item);
                await yield.ReturnAsync(result, cancellationToken2);
            }, cancellationToken);
        }

        private Task SelectImplIAsync(
            IAsyncYield<TResult> yield,
            CancellationToken cancellationToken)
        {
            var index = 0;
            return this.source.ForEachAsync(async (item, cancellationToken2) =>
            {
                var result = this.selectorI(item, index);
                await yield.ReturnAsync(result, cancellationToken2);
                index++;
            }, cancellationToken);
        }
    }
}
