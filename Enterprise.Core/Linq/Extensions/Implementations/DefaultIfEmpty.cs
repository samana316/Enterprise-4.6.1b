using System.Threading;
using System.Threading.Tasks;

namespace Enterprise.Core.Linq
{
    internal sealed class DefaultIfEmpty<TSource> : AsyncEnumerableBase<TSource>
    {
        private readonly IAsyncEnumerable<TSource> source;

        private readonly TSource defaultValue;

        public DefaultIfEmpty(
            IAsyncEnumerable<TSource> source, 
            TSource defaultValue)
        {
            this.source = source;
            this.defaultValue = defaultValue;
        }

        public override AsyncIterator<TSource> Clone()
        {
            return new DefaultIfEmpty<TSource>(this.source, this.defaultValue);
        }

        protected override async Task EnumerateAsync(
            IAsyncYield<TSource> yield, 
            CancellationToken cancellationToken)
        {
            using (var enumerator = source.GetAsyncEnumerator())
            {
                if (!await enumerator.MoveNextAsync(cancellationToken))
                {
                    await yield.ReturnAsync(this.defaultValue, cancellationToken);
                    yield.Break();
                }

                do
                {
                    await yield.ReturnAsync(enumerator.Current, cancellationToken);
                } while (await enumerator.MoveNextAsync(cancellationToken));
            }
        }
    }
}
