using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;

namespace Enterprise.Core.Reactive.Linq.Implementations
{
    internal sealed class Amb<TSource> : AsyncObservableBase<TSource>
    {
        private readonly IEnumerable<IAsyncObservable<TSource>> sources;

        public Amb(
            IEnumerable<IAsyncObservable<TSource>> sources)
        {
            this.sources = sources;
        }

        public override AsyncIterator<TSource> Clone()
        {
            return new Amb<TSource>(this.sources);
        }

        protected override async Task ProduceAsync(
            IAsyncYield<TSource> yield, 
            CancellationToken cancellationToken)
        {
            var tasks = this.sources.Select(this.Decide);
            var decision = await Task.WhenAny(tasks).Unwrap();

            await yield.ReturnAllAsync(decision, cancellationToken);
        }

        private async Task<IAsyncObservable<TSource>> Decide(
            IAsyncObservable<TSource> source)
        {
            await source.Take(1);

            return source;
        }
    }
}
