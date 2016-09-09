using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;

namespace Enterprise.Core.Reactive.Linq.Implementations
{
    internal sealed class Empty<TResult> : AsyncObservableBase<TResult>
    {
        public static Empty<TResult> Instance = new Empty<TResult>();

        private Empty()
        {
        }

        public override AsyncIterator<TResult> Clone()
        {
            return this;
        }

        protected override async Task ProduceAsync(
            IAsyncYield<TResult> yield, 
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await Task.Yield();
        }
    }
}
