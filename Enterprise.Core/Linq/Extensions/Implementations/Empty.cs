using System.Threading;
using System.Threading.Tasks;

namespace Enterprise.Core.Linq
{
    internal sealed class Empty<TResult> : AsyncEnumerableBase<TResult>
    {
        public static Empty<TResult> Instance = new Empty<TResult>();

        private Empty()
        {
        }

        public override AsyncIterator<TResult> Clone()
        {
            return this;
        }

        protected override async Task EnumerateAsync(
            IAsyncYield<TResult> yield, 
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Yield();

            yield.Break();
        }
    }
}
