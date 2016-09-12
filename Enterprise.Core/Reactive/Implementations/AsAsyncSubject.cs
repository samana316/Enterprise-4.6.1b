using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Core.Reactive.Linq;

namespace Enterprise.Core.Reactive
{
    internal sealed class AsAsyncSubject<TSource> : AsyncSubjectBase<TSource>
    {
        private readonly IAsyncObservable<TSource> source;

        public AsAsyncSubject(
            IAsyncObservable<TSource> source)
        {
            this.source = source;
        }

        public override AsyncIterator<TSource> Clone()
        {
            return new AsAsyncSubject<TSource>(this.source);
        }

        protected override Task ProduceAsync(
            IAsyncYield<TSource> yield, 
            CancellationToken cancellationToken)
        {
            return this.source.ForEachAsync((item, cancellationToken2) =>
            {
                return yield.ReturnAsync(item, cancellationToken2);
            }, cancellationToken);
        }
    }
}
