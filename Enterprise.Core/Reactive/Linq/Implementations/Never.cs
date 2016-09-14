using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;

namespace Enterprise.Core.Reactive.Linq.Implementations
{
    internal sealed class Never<TResult> : AsyncObservableBase<TResult>
    {
        public static Never<TResult> Instance = new Never<TResult>();

        private Never()
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
            await Task.Yield();

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }
        }
    }
}
