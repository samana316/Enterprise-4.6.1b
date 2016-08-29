using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Enterprise.Core.Linq
{
    internal sealed class GroupJoin<TOuter, TInner, TKey, TResult> : AsyncEnumerableBase<TResult>
    {
        private readonly IAsyncEnumerable<TOuter> outer;

        private readonly IAsyncEnumerable<TInner> inner;

        private readonly Func<TOuter, TKey> outerKeySelector;

        private readonly Func<TInner, TKey> innerKeySelector;

        private readonly Func<TOuter, IEnumerable<TInner>, TResult> resultSelector;

        private readonly IEqualityComparer<TKey> comparer;

        public GroupJoin(
            IAsyncEnumerable<TOuter> outer,
            IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, IEnumerable<TInner>, TResult> resultSelector,
            IEqualityComparer<TKey> comparer)
        {
            this.outer = outer;
            this.inner = inner.AsAsyncEnumerable();
            this.outerKeySelector = outerKeySelector;
            this.innerKeySelector = innerKeySelector;
            this.resultSelector = resultSelector;
            this.comparer = comparer ?? EqualityComparer<TKey>.Default;
        }

        public override AsyncIterator<TResult> Clone()
        {
            return new GroupJoin<TOuter, TInner, TKey, TResult>(
                this.outer,
                this.inner,
                this.outerKeySelector,
                this.innerKeySelector,
                this.resultSelector,
                this.comparer);
        }

        protected override async Task EnumerateAsync(
            IAsyncYield<TResult> yield,
            CancellationToken cancellationToken)
        {
            var lookup = await this.inner.ToLookupAsync(
                this.innerKeySelector, this.comparer, cancellationToken);

            await this.outer.ForEachAsync((outerElement, cancellationToken2) => 
            {
                var key = this.outerKeySelector(outerElement);
                var result = this.resultSelector(outerElement, lookup[key]);

                return yield.ReturnAsync(result, cancellationToken);
            }, cancellationToken);
        }
    }
}
