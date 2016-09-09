using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;

namespace Enterprise.Core.Reactive.Linq.Implementations
{
    internal sealed class Anonymous<TSource> : AsyncObservableBase<TSource>
    {
        private readonly Func<IAsyncYield<TSource>, CancellationToken, Task> producer;

        public Anonymous(
            Func<IAsyncYield<TSource>, CancellationToken, Task> producer)
        {
            this.producer = producer;
        }

        public override AsyncIterator<TSource> Clone()
        {
            return new Anonymous<TSource>(this.producer);
        }

        protected override Task ProduceAsync(
            IAsyncYield<TSource> yield, 
            CancellationToken cancellationToken)
        {
            return this.producer(yield, cancellationToken);
        }
    }
}
