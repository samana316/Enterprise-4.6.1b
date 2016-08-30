using System.Threading;
using System.Threading.Tasks;

namespace Enterprise.Core.Linq
{
    internal sealed class Take<TSource> : AsyncEnumerableBase<TSource>
    {
        private readonly IAsyncEnumerable<TSource> source;

        private readonly int count;

        public Take(
            IAsyncEnumerable<TSource> source, 
            int count)
        {
            this.source = source;
            this.count = count;
        }

        public override AsyncIterator<TSource> Clone()
        {
            return new Take<TSource>(this.source, this.count);
        }

        protected override async Task EnumerateAsync(
            IAsyncYield<TSource> yield, 
            CancellationToken cancellationToken)
        {
            using (var iterator = this.source.GetAsyncEnumerator())
            {
                for (int i = 0; i < this.count && await iterator.MoveNextAsync(cancellationToken); i++)
                {
                    await yield.ReturnAsync(iterator.Current, cancellationToken);
                }
            }
        }
    }
}
