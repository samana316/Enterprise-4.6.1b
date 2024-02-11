using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;

namespace Enterprise.Core.Reactive.Linq.Implementations
{
    internal sealed class SequenceEqual<TSource> : AsyncObservableBase<bool>
    {
        private readonly IAsyncObservable<TSource> first;

        private readonly IAsyncObservable<TSource> second;

        private readonly IAsyncEnumerable<TSource> secondE;

        private readonly IEqualityComparer<TSource> comparer;

        public SequenceEqual(
            IAsyncObservable<TSource> first,
            IObservable<TSource> second,
            IEqualityComparer<TSource> comparer)
        {
            throw new NotImplementedException();
        }

        public SequenceEqual(
            IAsyncObservable<TSource> first,
            IEnumerable<TSource> second,
            IEqualityComparer<TSource> comparer)
        {
            this.first = first;
            this.secondE = second.AsAsyncEnumerable();
            this.comparer = comparer ?? EqualityComparer<TSource>.Default;
        }

        public override AsyncIterator<bool> Clone()
        {
            return new SequenceEqual<TSource>(this.first, this.secondE, this.comparer);
        }

        protected override async Task ProduceAsync(
            IAsyncYield<bool> yield, 
            CancellationToken cancellationToken)
        {
            var result = true;

            using (var iterator2 = secondE.GetAsyncEnumerator())
            {
                await first.ForEachAsync(async (item1, cancellationToken2) =>
                {
                    if (!await iterator2.MoveNextAsync(cancellationToken2))
                    {
                        result = false;
                    }

                    if (!comparer.Equals(item1, iterator2.Current))
                    {
                        result = false;
                    }
                }, cancellationToken);

                result = !await iterator2.MoveNextAsync(cancellationToken);
            }

            await yield.ReturnAsync(result, cancellationToken);
        }
    }
}
