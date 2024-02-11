using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;

namespace Enterprise.Core.Reactive.Linq.Implementations
{
    internal sealed class StartWith<TSource> : AsyncObservableBase<TSource>
    {
        private readonly IAsyncObservable<TSource> sources;

        private readonly IEnumerable<TSource> values;

        public StartWith(
            IAsyncObservable<TSource> sources, 
            IEnumerable<TSource> values)
        {
            this.sources = sources;
            this.values = values;
        }

        public override AsyncIterator<TSource> Clone()
        {
            return new StartWith<TSource>(this.sources, this.values);
        }

        protected override async Task ProduceAsync(
            IAsyncYield<TSource> yield, 
            CancellationToken cancellationToken)
        {
            await yield.ReturnAllAsync(this.values, cancellationToken);
            await yield.ReturnAllAsync(this.sources, cancellationToken);
        }
    }
}
