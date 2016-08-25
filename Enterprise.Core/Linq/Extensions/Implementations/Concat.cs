using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Enterprise.Core.Linq
{
    internal sealed class Concat<TSource> : AsyncEnumerableBase<TSource>
    {
        private readonly IAsyncEnumerable<TSource> first;

        private readonly IEnumerable<TSource> second;

        public Concat(
            IAsyncEnumerable<TSource> first, 
            IEnumerable<TSource> second)
        {
            this.first = first;
            this.second = second;
        }

        public override AsyncIterator<TSource> Clone()
        {
            return new Concat<TSource>(this.first, this.second);
        }

        protected override async Task EnumerateAsync(
            IAsyncYield<TSource> yield, 
            CancellationToken cancellationToken)
        {
            await yield.ReturnAllAsync(this.first, cancellationToken);
            await yield.ReturnAllAsync(this.second, cancellationToken);
        }
    }
}
