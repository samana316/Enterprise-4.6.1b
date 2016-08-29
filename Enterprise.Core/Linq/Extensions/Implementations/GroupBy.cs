using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Enterprise.Core.Linq
{
    internal sealed class GroupBy<TSource, TKey, TElement> : AsyncEnumerableBase<IGrouping<TKey, TElement>>
    {
        private readonly IAsyncEnumerable<TSource> source;

        private readonly Func<TSource, TKey> keySelector;

        private readonly Func<TSource, TElement> elementSelector;

        private readonly IEqualityComparer<TKey> comparer;

        public GroupBy(
            IAsyncEnumerable<TSource> source, 
            Func<TSource, TKey> keySelector, 
            Func<TSource, TElement> elementSelector, 
            IEqualityComparer<TKey> comparer)
        {
            this.source = source;
            this.keySelector = keySelector;
            this.elementSelector = elementSelector;
            this.comparer = comparer ?? EqualityComparer<TKey>.Default;
        }

        public override AsyncIterator<IGrouping<TKey, TElement>> Clone()
        {
            return new GroupBy<TSource, TKey, TElement>(
                this.source, this.keySelector, this.elementSelector, this.comparer);
        }

        protected override async Task EnumerateAsync(
            IAsyncYield<IGrouping<TKey, TElement>> yield, 
            CancellationToken cancellationToken)
        {
            var lookup = await source.ToLookupAsync(
                this.keySelector, this.elementSelector, this.comparer, cancellationToken);

            await yield.ReturnAllAsync(lookup, cancellationToken);
        }
    }
}
