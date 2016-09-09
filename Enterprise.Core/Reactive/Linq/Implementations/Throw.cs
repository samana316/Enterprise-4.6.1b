using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;

namespace Enterprise.Core.Reactive.Linq.Implementations
{
    internal sealed class Throw<TResult> : AsyncObservableBase<TResult>
    {
        private readonly Exception exception;

        public Throw(
            Exception exception)
        {
            this.exception = exception;
        }

        public override AsyncIterator<TResult> Clone()
        {
            return new Throw<TResult>(exception);
        }

        protected override Task ProduceAsync(
            IAsyncYield<TResult> yield, 
            CancellationToken cancellationToken)
        {
            throw this.exception;
        }
    }
}
