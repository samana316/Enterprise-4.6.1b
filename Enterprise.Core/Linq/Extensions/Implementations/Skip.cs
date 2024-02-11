using System.Threading;
using System.Threading.Tasks;

namespace Enterprise.Core.Linq
{
    internal sealed class Skip<TSource> : AsyncEnumerableBase<TSource>
    {
        private readonly IAsyncEnumerable<TSource> source;

        private readonly int count;

        public Skip(
            IAsyncEnumerable<TSource> source,
            int count)
        {
            this.source = source;
            this.count = count;
        }

        public override AsyncIterator<TSource> Clone()
        {
            return new Skip<TSource>(this.source, this.count);
        }

        protected override async Task EnumerateAsync(
            IAsyncYield<TSource> yield,
            CancellationToken cancellationToken)
        {
            using (var iterator = this.source.GetAsyncEnumerator())
            {
                for (int i = 0; i < this.count; i++)
                {
                    if (!await iterator.MoveNextAsync(cancellationToken))
                    {
                        yield.Break();
                    }
                }

                while (await iterator.MoveNextAsync(cancellationToken))
                {
                    await yield.ReturnAsync(iterator.Current, cancellationToken);
                }
            }
        }
    }
}
