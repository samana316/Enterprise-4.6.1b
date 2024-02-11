using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;

namespace Enterprise.Core.Reactive.Linq.Implementations
{
    internal sealed class SelectMany<TSource, TCollection, TResult> : AsyncObservableBase<TResult>
    {
        private readonly IAsyncObservable<TSource> source;

        private readonly Func<TSource, IObservable<TCollection>> collectionSelector;

        private readonly Func<TSource, int, IObservable<TCollection>> collectionSelectorI;

        private readonly Func<TSource, IEnumerable<TCollection>> collectionSelectorE;

        private readonly Func<TSource, int, IEnumerable<TCollection>> collectionSelectorEI;

        private readonly Func<TSource, TCollection, TResult> resultSelector;

        private readonly Func<TSource, int, TCollection, int, TResult> resultSelectorI;

        private readonly Func<TSource, CancellationToken, Task<TCollection>> collectionSelectorT;

        private readonly Func<TSource, int, CancellationToken, Task<TCollection>> collectionSelectorTI;

        private readonly Func<TSource, int, TCollection, TResult> resultSelectorTI;

        private readonly IAsyncYieldBuilder<TResult> producer;

        private SelectMany(
            SelectMany<TSource, TCollection, TResult> parent)
        {
            this.source = parent.source;
            this.collectionSelector = parent.collectionSelector;
            this.collectionSelectorI = parent.collectionSelectorI;
            this.collectionSelectorE = parent.collectionSelectorE;
            this.collectionSelectorEI = parent.collectionSelectorEI;
            this.resultSelector = parent.resultSelector;
            this.resultSelectorI = parent.resultSelectorI;
            this.collectionSelectorT = parent.collectionSelectorT;
            this.collectionSelectorTI = parent.collectionSelectorTI;
            this.resultSelectorTI = parent.resultSelectorTI;
            this.producer = parent.producer;
        }

        public SelectMany(
            IAsyncObservable<TSource> source,
            Func<TSource, IObservable<TCollection>> collectionSelector,
            Func<TSource, TCollection, TResult> resultSelector)
        {
            this.source = source;
            this.collectionSelector = collectionSelector;
            this.resultSelector = resultSelector;
            this.producer = new SelectManyImpl(this);
        }

        public SelectMany(
            IAsyncObservable<TSource> source,
            Func<TSource, int, IObservable<TCollection>> collectionSelector,
            Func<TSource, int, TCollection, int, TResult> resultSelector)
        {
            this.source = source;
            this.collectionSelectorI = collectionSelector;
            this.resultSelectorI = resultSelector;
            this.producer = new SelectManyImplI(this);
        }

        public SelectMany(
            IAsyncObservable<TSource> source,
            Func<TSource, IEnumerable<TCollection>> collectionSelector,
            Func<TSource, TCollection, TResult> resultSelector)
        {
            this.source = source;
            this.collectionSelectorE = collectionSelector;
            this.resultSelector = resultSelector;
            this.producer = new SelectManyImpl(this);
        }

        public SelectMany(
            IAsyncObservable<TSource> source,
            Func<TSource, int, IEnumerable<TCollection>> collectionSelector,
            Func<TSource, int, TCollection, int, TResult> resultSelector)
        {
            this.source = source;
            this.collectionSelectorEI = collectionSelector;
            this.resultSelectorI = resultSelector;
            this.producer = new SelectManyImplI(this);
        }

        public SelectMany(
            IAsyncObservable<TSource> source,
            Func<TSource, CancellationToken, Task<TCollection>> collectionSelector,
            Func<TSource, TCollection, TResult> resultSelector)
        {
            this.source = source;
            this.collectionSelectorT = collectionSelector;
            this.resultSelector = resultSelector;
            this.producer = new SelectManyImplT(this);
        }

        public SelectMany(
            IAsyncObservable<TSource> source,
            Func<TSource, int, CancellationToken, Task<TCollection>> collectionSelector,
            Func<TSource, int, TCollection, TResult> resultSelector)
        {
            this.source = source;
            this.collectionSelectorTI = collectionSelector;
            this.resultSelectorTI = resultSelector;
            this.producer = new SelectManyImplTI(this);
        }

        public override AsyncIterator<TResult> Clone()
        {
            return new SelectMany<TSource, TCollection, TResult>(this);
        }

        protected override Task ProduceAsync(
            IAsyncYield<TResult> yield, 
            CancellationToken cancellationToken)
        {
            return this.producer.RunAsync(yield, cancellationToken);
        }

        private sealed class SelectManyImpl : IAsyncYieldBuilder<TResult>
        {
            private readonly SelectMany<TSource, TCollection, TResult> parent;

            public SelectManyImpl(
                SelectMany<TSource, TCollection, TResult> parent)
            {
                this.parent = parent;
            }

            public async Task RunAsync(
                IAsyncYield<TResult> yield, 
                CancellationToken cancellationToken)
            {
                var tasks = new List<Task>();

                await this.parent.source.ForEachAsync((item, cancellationToken2) =>
                {
                    var results =
                        from collectionItem in this.GetIntermediateSequence(item)
                        select this.parent.resultSelector(item, collectionItem);

                    tasks.Add(yield.ReturnAllAsync(results, cancellationToken2));

                    return Task.CompletedTask;
                }, cancellationToken);

                await Task.WhenAll(tasks);
            }

            private IAsyncObservable<TCollection> GetIntermediateSequence(
                TSource item)
            {
                if (this.parent.collectionSelectorE != null)
                {
                    return this.parent.collectionSelectorE(item).ToAsyncObservable();
                }

                return this.parent.collectionSelector(item).AsAsyncObservable();
            }
        }

        private sealed class SelectManyImplI : IAsyncYieldBuilder<TResult>
        {
            private readonly SelectMany<TSource, TCollection, TResult> parent;

            public SelectManyImplI(
                SelectMany<TSource, TCollection, TResult> parent)
            {
                this.parent = parent;
            }

            public async Task RunAsync(
                IAsyncYield<TResult> yield, 
                CancellationToken cancellationToken)
            {
                var tasks = new List<Task>();
                var index = 0;

                await this.parent.source.ForEachAsync((item, cancellationToken2) =>
                {
                    var cachedIndex = index++;
                    var collectionIndex = 0;
                    var collection = this.GetIntermediateSequence(item, cachedIndex);

                    var task = collection.ForEachAsync((collectionItem, cancellationToken3) => 
                    {
                        var result = this.parent.resultSelectorI(
                            item, cachedIndex, collectionItem, collectionIndex);

                        collectionIndex++;
                        return yield.ReturnAsync(result, cancellationToken3);
                    }, cancellationToken2);
                    
                    tasks.Add(task);

                    return Task.CompletedTask;
                }, cancellationToken);

                await Task.WhenAll(tasks);
            }

            private IAsyncObservable<TCollection> GetIntermediateSequence(
                TSource item,
                int index)
            {
                if (this.parent.collectionSelectorEI != null)
                {
                    return this.parent.collectionSelectorEI(item, index).ToAsyncObservable();
                }

                return this.parent.collectionSelectorI(item, index).AsAsyncObservable();
            }
        }

        private sealed class SelectManyImplT : IAsyncYieldBuilder<TResult>
        {
            private readonly SelectMany<TSource, TCollection, TResult> parent;

            public SelectManyImplT(
                SelectMany<TSource, TCollection, TResult> parent)
            {
                this.parent = parent;
            }

            public async Task RunAsync(
                IAsyncYield<TResult> yield, 
                CancellationToken cancellationToken)
            {
                var tasks = new List<Task>();

                await this.parent.source.ForEachAsync((item, cancellationToken2) =>
                {
                    Func<Task> function = async () =>
                    {
                        var task = this.parent.collectionSelectorT(item, cancellationToken2);
                        var result = this.parent.resultSelector(item, await task);

                        await yield.ReturnAsync(result, cancellationToken2);
                    };

                    tasks.Add(function());

                    return Task.CompletedTask;
                }, cancellationToken);

                await Task.WhenAll(tasks);
            }
        }

        private sealed class SelectManyImplTI : IAsyncYieldBuilder<TResult>
        {
            private readonly SelectMany<TSource, TCollection, TResult> parent;

            public SelectManyImplTI(
                SelectMany<TSource, TCollection, TResult> parent)
            {
                this.parent = parent;
            }

            public async Task RunAsync(
                IAsyncYield<TResult> yield,
                CancellationToken cancellationToken)
            {
                var tasks = new List<Task>();
                var index = 0;

                await this.parent.source.ForEachAsync((item, cancellationToken2) =>
                {
                    var cachedIndex = index++;
                    Func<Task> function = async () =>
                    {
                        var task = this.parent.collectionSelectorTI(item, cachedIndex, cancellationToken2);
                        var result = this.parent.resultSelectorTI(item, cachedIndex, await task);

                        await yield.ReturnAsync(result, cancellationToken2);
                    };

                    tasks.Add(function());

                    return Task.CompletedTask;
                }, cancellationToken);

                await Task.WhenAll(tasks);
            }
        }
    }
}
