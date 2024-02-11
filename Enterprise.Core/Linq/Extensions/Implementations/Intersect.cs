using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Enterprise.Core.Linq
{
    internal sealed class Intersect<TSource> : AsyncEnumerableBase<TSource>
    {
        private readonly IAsyncEnumerable<TSource> first;

        private readonly IEnumerable<TSource> second;

        private readonly IEqualityComparer<TSource> comparer;

        public Intersect(
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
            return new Intersect<TSource>(this.first, this.second, this.comparer);
        }

        protected override async Task EnumerateAsync(
            IAsyncYield<TSource> yield,
            CancellationToken cancellationToken)
        {
            var set = new HashSet<TSource>(this.comparer);

            await this.second.AsAsyncEnumerable().ForEachAsync(item =>
            {
                set.Add(item);
            }, cancellationToken);

            await this.first.ForEachAsync(async (item, cancellationToken2) =>
            {
                if (set.Remove(item))
                {
                    await yield.ReturnAsync(item, cancellationToken2);
                }
            }, cancellationToken);
        }
    }
}
