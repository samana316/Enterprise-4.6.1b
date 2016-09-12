using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;

namespace Enterprise.Core.Reactive.Linq.Implementations
{
    internal sealed class DoWhile<TSource> : AsyncObservableBase<TSource>
    {
        private readonly IAsyncObservable<TSource> source;

        private readonly Func<bool> condition;

        public DoWhile(
            IAsyncObservable<TSource> source,
            Func<bool> condition)
        {
            this.source = source;
            this.condition = condition;
        }

        public override AsyncIterator<TSource> Clone()
        {
            return new DoWhile<TSource>(this.source, this.condition);
        }

        protected override Task ProduceAsync(
            IAsyncYield<TSource> yield,
            CancellationToken cancellationToken)
        {
            var first = true;
            return this.source.ForEachAsync(async (item, cancellationToken2) =>
            {
                if (!this.condition() && !first)
                {
                    yield.Break();
                }

                first = false;
                await yield.ReturnAsync(item, cancellationToken2);
            }, cancellationToken);
        }
    }
}
