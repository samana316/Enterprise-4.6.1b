using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;

namespace Enterprise.Core.Reactive.Linq.Implementations
{
    internal sealed class Finally<TSource> : AsyncObservableBase<TSource>
    {
        private readonly Action finallyAction;

        private readonly IAsyncObservable<TSource> source;

        public Finally(
            IAsyncObservable<TSource> source, 
            Action finallyAction)
        {
            this.source = source;
            this.finallyAction = finallyAction;
        }

        public override AsyncIterator<TSource> Clone()
        {
            return new Finally<TSource>(this.source, this.finallyAction);
        }

        protected override async Task ProduceAsync(
            IAsyncYield<TSource> yield, 
            CancellationToken cancellationToken)
        {
            try
            {
                await yield.ReturnAllAsync(this.source, cancellationToken);
                yield.Break();
            }
            finally
            {
                this.finallyAction();
            }
        }
    }
}
