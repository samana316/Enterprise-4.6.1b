using System.Threading;
using System.Threading.Tasks;

namespace Enterprise.Core.Linq
{
    internal sealed class Reverse<TSource> : AsyncEnumerableBase<TSource>
    {
        private readonly IAsyncEnumerable<TSource> source;

        public Reverse(
            IAsyncEnumerable<TSource> source)
        {
            this.source = source;
        }

        public override AsyncIterator<TSource> Clone()
        {
            return new Reverse<TSource>(this.source);
        }

        protected override async Task EnumerateAsync(
            IAsyncYield<TSource> yield,
            CancellationToken cancellationToken)
        {
            var buffer = await this.source.ToBufferAsync(cancellationToken);
            var count = buffer.Count;
            var array = buffer.Array;

            for (int i = count - 1; i >= 0; i--)
            {
                await yield.ReturnAsync(array[i], cancellationToken);
            }
        }
    }
}
