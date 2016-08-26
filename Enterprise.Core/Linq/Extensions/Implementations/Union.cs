using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Enterprise.Core.Linq
{
    internal sealed class Union<TSource> : AsyncEnumerableBase<TSource>
    {
        private readonly IAsyncEnumerable<TSource> first;

        private readonly IEnumerable<TSource> second;

        private readonly IEqualityComparer<TSource> comparer;

        public Union(
            IAsyncEnumerable<TSource> first, 
            IEnumerable<TSource> second, 
            IEqualityComparer<TSource> comparer)
        {
            this.first = first;
            this.second = second;
            this.comparer = comparer ?? EqualityComparer<TSource>.Default;
        }

        public override AsyncIterator<TSource> Clone()
        {
            return new Union<TSource>(this.first, this.second, this.comparer);
        }

        protected override async Task EnumerateAsync(
            IAsyncYield<TSource> yield, 
            CancellationToken cancellationToken)
        {
            var set = new HashSet<TSource>(this.comparer);

            await this.first.ForEachAsync(async (item, cancellationToken2) => 
            {
                if (set.Add(item))
                {
                    await yield.ReturnAsync(item, cancellationToken2);
                }
            }, cancellationToken);

            await this.second.AsAsyncEnumerable().ForEachAsync(async (item, cancellationToken2) =>
            {
                if (set.Add(item))
                {
                    await yield.ReturnAsync(item, cancellationToken2);
                }
            }, cancellationToken);
        }
    }
}
