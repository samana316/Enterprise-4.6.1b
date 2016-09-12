using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;

namespace Enterprise.Core.Reactive.Linq.Implementations
{
    internal sealed class While<TSource> : AsyncObservableBase<TSource>
    {
        private readonly IAsyncObservable<TSource> source;

        private readonly Func<bool> condition;

        public While(
            IAsyncObservable<TSource> source, 
            Func<bool> condition)
        {
            this.source = source;
            this.condition = condition;
        }

        public override AsyncIterator<TSource> Clone()
        {
            return new While<TSource>(source, condition);
        }

        protected override Task ProduceAsync(
            IAsyncYield<TSource> yield, 
            CancellationToken cancellationToken)
        {
            return this.source.ForEachAsync(async (item, cancellationToken2) => 
            {
                if (!condition())
                {
                    yield.Break();
                }

                await yield.ReturnAsync(item, cancellationToken2);
            }, cancellationToken);
        }
    }
}
