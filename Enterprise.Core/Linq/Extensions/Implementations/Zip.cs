using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Enterprise.Core.Linq
{
    internal sealed class Zip<TFirst, TSecond, TResult> : AsyncEnumerableBase<TResult>
    {
        private readonly IAsyncEnumerable<TFirst> first;

        private readonly IAsyncEnumerable<TSecond> second;

        private readonly Func<TFirst, TSecond, TResult> resultSelector;

        public Zip(
            IAsyncEnumerable<TFirst> first, 
            IEnumerable<TSecond> second, 
            Func<TFirst, TSecond, TResult> resultSelector)
        {
            this.first = first;
            this.second = second.AsAsyncEnumerable();
            this.resultSelector = resultSelector;
        }

        public override AsyncIterator<TResult> Clone()
        {
            return new Zip<TFirst, TSecond, TResult>(this.first, this.second, this.resultSelector);
        }

        protected override async Task EnumerateAsync(
            IAsyncYield<TResult> yield, 
            CancellationToken cancellationToken)
        {
            using (var iterator1 = this.first.GetAsyncEnumerator())
            using (var iterator2 = this.second.GetAsyncEnumerator())
            {
                while (await iterator1.MoveNextAsync(cancellationToken) && 
                    await iterator2.MoveNextAsync(cancellationToken))
                {
                    var result = this.resultSelector(iterator1.Current, iterator2.Current);

                    await yield.ReturnAsync(result, cancellationToken);
                }
            }
        }
    }
}
