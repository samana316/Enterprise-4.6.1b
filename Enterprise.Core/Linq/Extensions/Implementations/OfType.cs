using System.Threading;
using System.Threading.Tasks;

namespace Enterprise.Core.Linq
{
    internal sealed class OfType<TResult> : AsyncEnumerableBase<TResult>
    {
        private readonly IAsyncEnumerable source;

        public OfType(
            IAsyncEnumerable source)
        {
            this.source = source;
        }

        public override AsyncIterator<TResult> Clone()
        {
            return new OfType<TResult>(this.source);
        }

        protected override Task EnumerateAsync(
            IAsyncYield<TResult> yield,
            CancellationToken cancellationToken)
        {
            return this.source.ForEachAsync(async (item, cancellationToken2) =>
            {
                if (item is TResult)
                {
                    await yield.ReturnAsync((TResult)item, cancellationToken2);
                }
            }, cancellationToken);
        }
    }
}
