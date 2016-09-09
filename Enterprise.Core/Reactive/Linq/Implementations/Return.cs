using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;

namespace Enterprise.Core.Reactive.Linq.Implementations
{
    internal sealed class Return<TResult> : AsyncObservableBase<TResult>
    {
        private readonly TResult value;

        public Return(
            TResult value)
        {
            this.value = value;
        }

        public override AsyncIterator<TResult> Clone()
        {
            return new Return<TResult>(this.value);
        }

        protected override Task ProduceAsync(
            IAsyncYield<TResult> yield, 
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return yield.ReturnAsync(this.value, cancellationToken);
        }
    }
}
