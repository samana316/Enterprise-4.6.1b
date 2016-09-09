using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;

namespace Enterprise.Core.Reactive.Linq.Implementations
{
    internal sealed class ToAsyncObservable<TSource> : AsyncObservableBase<TSource>
    {
        private readonly IEnumerable<TSource> source;

        public ToAsyncObservable(
            IEnumerable<TSource> source)
        {
            this.source = source.AsAsyncEnumerable();
        }

        public override AsyncIterator<TSource> Clone()
        {
            return new ToAsyncObservable<TSource>(this.source);
        }

        protected override Task ProduceAsync(
            IAsyncYield<TSource> yield, 
            CancellationToken cancellationToken)
        {
            return yield.ReturnAllAsync(this.source, cancellationToken);
        }
    }
}
