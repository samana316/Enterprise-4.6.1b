using System;
using System.Threading;
using System.Threading.Tasks;

namespace Enterprise.Core.Linq
{
    internal sealed class Anonymous<TSource> : AsyncEnumerableBase<TSource>
    {
        private readonly Func<IAsyncYield<TSource>, CancellationToken, Task> producer;

        public Anonymous(
            Func<IAsyncYield<TSource>, CancellationToken, Task> producer)
            : base()
        {
            this.producer = producer;
        }

        public override AsyncIterator<TSource> Clone()
        {
            return new Anonymous<TSource>(this.producer);
        }

        protected override Task EnumerateAsync(
            IAsyncYield<TSource> yield, 
            CancellationToken cancellationToken)
        {
            return this.producer(yield, cancellationToken);
        }
    }
}
