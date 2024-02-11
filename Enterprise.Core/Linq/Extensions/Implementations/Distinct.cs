using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Enterprise.Core.Linq
{
    internal sealed class Distinct<TSource> : AsyncEnumerableBase<TSource>
    {
        private readonly IAsyncEnumerable<TSource> source;

        private readonly IEqualityComparer<TSource> comparer;

        public Distinct(
            IAsyncEnumerable<TSource> source, 
            IEqualityComparer<TSource> comparer)
        {
            this.source = source;
            this.comparer = comparer ?? EqualityComparer<TSource>.Default;
        }

        public override AsyncIterator<TSource> Clone()
        {
            return new Distinct<TSource>(this.source, this.comparer);
        }

        protected override Task EnumerateAsync(
            IAsyncYield<TSource> yield, 
            CancellationToken cancellationToken)
        {
            var set = new HashSet<TSource>(this.comparer);

            return this.source.ForEachAsync(async (item, cancellationToken2) => 
            {
                if (set.Add(item))
                {
                    await yield.ReturnAsync(item, cancellationToken2);
                }
            }, cancellationToken);
        }
    }
}
