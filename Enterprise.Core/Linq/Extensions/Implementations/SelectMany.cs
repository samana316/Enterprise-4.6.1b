using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Enterprise.Core.Linq
{
    internal sealed class SelectMany<TSource, TResult> : AsyncEnumerableBase<TResult>
    {
        private readonly IAsyncEnumerable<TSource> source;

        private readonly Func<TSource, IEnumerable<TResult>> selector;

        private readonly Func<TSource, int, IEnumerable<TResult>> selectorI;

        public SelectMany(
            IAsyncEnumerable<TSource> source, 
            Func<TSource, IEnumerable<TResult>> selector)
        {
            this.source = source;
            this.selector = selector;
        }

        public SelectMany(
            IAsyncEnumerable<TSource> source, 
            Func<TSource, int, IEnumerable<TResult>> selector)
        {
            this.source = source;
            this.selectorI = selector;
        }

        public override AsyncIterator<TResult> Clone()
        {
            if (this.selectorI == null)
            {
                return new SelectMany<TSource, TResult>(this.source, this.selector);
            }

            return new SelectMany<TSource, TResult>(this.source, this.selectorI);
        }

        protected override Task EnumerateAsync(
            IAsyncYield<TResult> yield, 
            CancellationToken cancellationToken)
        {
            var index = 0;

            return this.source.ForEachAsync((item, cancellationToken2) =>
            {
                var results = this.selectorI == null ?
                    this.selector(item) : this.selectorI(item, index++);

                return yield.ReturnAllAsync(results, cancellationToken2);
            }, cancellationToken);
        }
    }

    internal sealed class SelectMany<TSource, TCollection, TResult> : AsyncEnumerableBase<TResult>
    {
        private readonly IAsyncEnumerable<TSource> source;

        private readonly Func<TSource, IEnumerable<TCollection>> collectionSelector;

        private readonly Func<TSource, int, IEnumerable<TCollection>> collectionSelectorI;

        private readonly Func<TSource, TCollection, TResult> resultSelector;

        public SelectMany(
            IAsyncEnumerable<TSource> source, 
            Func<TSource, IEnumerable<TCollection>> collectionSelector, 
            Func<TSource, TCollection, TResult> resultSelector)
        {
            this.source = source;
            this.collectionSelector = collectionSelector;
            this.resultSelector = resultSelector;
        }

        public SelectMany(
            IAsyncEnumerable<TSource> source, 
            Func<TSource, int, IEnumerable<TCollection>> collectionSelector, 
            Func<TSource, TCollection, TResult> resultSelector)
        {
            this.source = source;
            this.collectionSelectorI = collectionSelector;
            this.resultSelector = resultSelector;
        }

        public override AsyncIterator<TResult> Clone()
        {
            if (this.collectionSelectorI == null)
            {
                return new SelectMany<TSource, TCollection, TResult>(
                    this.source, this.collectionSelector, this.resultSelector);
            }

            return new SelectMany<TSource, TCollection, TResult>(
                this.source, this.collectionSelectorI, this.resultSelector);
        }

        protected override Task EnumerateAsync(
            IAsyncYield<TResult> yield, 
            CancellationToken cancellationToken)
        {
            var index = 0;

            return this.source.ForEachAsync((item, ct2) => 
            {
                var collection = this.collectionSelectorI == null ? 
                    this.collectionSelector(item) : this.collectionSelectorI(item, index++);

                var results = 
                    from collectionItem in collection.AsAsyncEnumerable()
                    select this.resultSelector(item, collectionItem);

                return yield.ReturnAllAsync(results, ct2);
            }, cancellationToken);
        }
    }
}
