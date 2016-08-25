using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Enterprise.Core.Linq
{
    internal sealed class AsAsyncEnumerable<TSource> : AsyncEnumerableBase<TSource>
    {
        private readonly IEnumerable<TSource> source;

        public AsAsyncEnumerable(
            IEnumerable<TSource> source)
        {
            this.source = source;
        }

        public override AsyncIterator<TSource> Clone()
        {
            return new AsAsyncEnumerable<TSource>(this.source);
        }

        protected override async Task EnumerateAsync(
            IAsyncYield<TSource> yield, 
            CancellationToken cancellationToken)
        {
            using (var enumerator = this.source.GetEnumerator())
            {
                while (await enumerator.MoveNextAsync(cancellationToken))
                {
                    await yield.ReturnAsync(enumerator.Current, cancellationToken);
                }
            }
        }
    }
}
