using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;

namespace Enterprise.Core.Reactive.Linq.Implementations
{
    internal sealed class ToList<TSource> : AsyncObservableBase<IList<TSource>>
    {
        private readonly IAsyncObservable<TSource> source;

        public ToList(
            IAsyncObservable<TSource> source)
        {
            this.source = source;
        }

        public override AsyncIterator<IList<TSource>> Clone()
        {
            return new ToList<TSource>(this.source);
        }

        protected override async Task ProduceAsync(
            IAsyncYield<IList<TSource>> yield, 
            CancellationToken cancellationToken)
        {
            var list = new List<TSource>();

            var collection = this.source as ICollection<TSource>;
            if (collection != null)
            {
                list = new List<TSource>(collection);
            }
            else
            {
                await this.source.ForEachAsync((item, cancellationToken2) =>
                {
                    return Task.Run(() => list.Add(item), cancellationToken2);
                }, cancellationToken);
            }

            await yield.ReturnAsync(list, cancellationToken);
        }
    }
}
