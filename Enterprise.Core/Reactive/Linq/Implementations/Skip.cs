using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;

namespace Enterprise.Core.Reactive.Linq.Implementations
{
    internal sealed class Skip<TSource> : AsyncObservableBase<TSource>
    {
        private readonly IAsyncObservable<TSource> source;

        private readonly int count;

        public Skip(
            IAsyncObservable<TSource> source,
            int count)
        {
            this.source = source;
            this.count = count;
        }

        public override AsyncIterator<TSource> Clone()
        {
            return new Skip<TSource>(this.source, this.count);
        }

        protected override Task ProduceAsync(
            IAsyncYield<TSource> yield,
            CancellationToken cancellationToken)
        {
            var i = 0;
            return this.source.ForEachAsync((item, cancellationToken2) => 
            {
                if (i < this.count)
                {
                    i++;
                }

                return yield.ReturnAsync(item, cancellationToken2);
            }, cancellationToken);
        }
    }
}
