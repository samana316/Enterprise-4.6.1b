using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;

namespace Enterprise.Core.Reactive.Linq.Implementations
{
    internal sealed class Take<TSource> : AsyncObservableBase<TSource>
    {
        private readonly IAsyncObservable<TSource> source;

        private readonly int count;

        public Take(
            IAsyncObservable<TSource> source,
            int count)
        {
            this.source = source;
            this.count = count;
        }

        public override AsyncIterator<TSource> Clone()
        {
            return new Take<TSource>(this.source, this.count);
        }

        protected override Task ProduceAsync(
            IAsyncYield<TSource> yield,
            CancellationToken cancellationToken)
        {
            var i = 0;
            return source.ForEachAsync(async (item, cancellationToken2) => 
            {
                if (i >= this.count)
                {
                    yield.Break();
                }

                i++;
                await yield.ReturnAsync(item, cancellationToken2);
            }, cancellationToken);
        }
    }
}
