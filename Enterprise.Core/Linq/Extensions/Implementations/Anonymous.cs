using System;
using System.Threading;
using System.Threading.Tasks;

namespace Enterprise.Core.Linq
{
    internal sealed class Anonymous<TSource> : AsyncEnumerableBase<TSource>
    {
        private readonly Func<IAsyncYield<TSource>, CancellationToken, Task> yieldBuilder;

        public Anonymous(
            Func<IAsyncYield<TSource>, CancellationToken, Task> yieldBuilder)
            : base()
        {
            this.yieldBuilder = yieldBuilder;
        }

        public override AsyncIterator<TSource> Clone()
        {
            return new Anonymous<TSource>(this.yieldBuilder);
        }

        protected override Task EnumerateAsync(
            IAsyncYield<TSource> yield, 
            CancellationToken cancellationToken)
        {
            return this.yieldBuilder(yield, cancellationToken);
        }
    }
}
