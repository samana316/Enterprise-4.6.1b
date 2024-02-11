using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;

namespace Enterprise.Core.Reactive.Linq.Implementations
{
    internal sealed class Zip<TSource> : AsyncObservableBase<IList<TSource>>
    {
        private readonly IAsyncEnumerable<IAsyncObservable<TSource>> sources;

        public Zip(
            IEnumerable<IAsyncObservable<TSource>> sources)
        {
            this.sources = sources.AsAsyncEnumerable();
        }

        public override AsyncIterator<IList<TSource>> Clone()
        {
            return new Zip<TSource>(this.sources);
        }

        protected override async Task ProduceAsync(
            IAsyncYield<IList<TSource>> yield, 
            CancellationToken cancellationToken)
        {
            var list = await this.sources.ToListAsync(cancellationToken);
            if (list.Count == 0)
            {
                yield.Break();
                return;
            }

            if (list.Count == 1)
            {
                var source = list[0];
                await source.ForEachAsync((item, cancellationToken2) => 
                {
                    return yield.ReturnAsync(new[] { item }, cancellationToken2);
                }, cancellationToken);

                yield.Break();
                return;
            }

            var producer = new ZipImpl(this, list);
            await producer.RunAsync(yield, cancellationToken);

            yield.Break();
        }

        private sealed class ZipImpl : IAsyncYieldBuilder<IList<TSource>>
        {
            private readonly Zip<TSource> parent;

            private readonly IReadOnlyCollection<IAsyncObservable<TSource>> sources;

            private IAsyncYield<IList<TSource>> yield;

            private ParallelResultCollection results;

            private int index;

            private int[] collectionIndexes;

            public ZipImpl(
                Zip<TSource> parent, 
                IReadOnlyCollection<IAsyncObservable<TSource>> sources)
            {
                this.parent = parent;
                this.sources = sources;
            }

            public Task RunAsync(
                IAsyncYield<IList<TSource>> yield, 
                CancellationToken cancellationToken)
            {
                var tasks = new List<Task>();
                this.yield = yield;
                this.results = new ParallelResultCollection(yield, this.sources.Count);
                this.collectionIndexes = new int[this.sources.Count];

                foreach (var source in this.sources)
                {
                    var cachedIndex = this.index++;
                    var task = this.RunCollectionAsync(source, cachedIndex, cancellationToken);

                    tasks.Add(task);
                }

                return Task.WhenAll(tasks);
            }

            private async Task RunCollectionAsync(
                IAsyncObservable<TSource> source,
                int cachedIndex,
                CancellationToken cancellationToken)
            {
                try
                {
                    await source.ForEachAsync((item, cancellationToken2) =>
                    {
                        if (this.results.ShouldComplete(this.collectionIndexes[cachedIndex]))
                        {
                            this.yield.Break();
                        }

                        var cachedCollectionIndex = this.collectionIndexes[cachedIndex]++;

                        return results.SetElementAsync(
                            item, cachedIndex, cachedCollectionIndex, cancellationToken2);

                    }, cancellationToken);
                }
                finally
                {
                    results.SetComplete(this.collectionIndexes[cachedIndex]);
                }
            }
        }

        private sealed class ParallelResultCollection
        {
            private readonly object sink = new object();
            
            private readonly IDictionary<int, ParallelResult> map = new SortedDictionary<int, ParallelResult>();

            private readonly IAsyncYield<IList<TSource>> yield;

            private readonly int count;

            private bool complete;

            private int terminatingIndex;

            public ParallelResultCollection(
                IAsyncYield<IList<TSource>> yield,
                int count)
            {
                this.yield = yield;
                this.count = count;
            }

            public async Task SetElementAsync(
                TSource element, 
                int index, 
                int collectionIndex, 
                CancellationToken cancellationToken)
            {
                ParallelResult item;
                lock (sink)
                {
                    if (!this.map.TryGetValue(collectionIndex, out item))
                    {
                        item = new ParallelResult(collectionIndex, this);
                        this.map.Add(collectionIndex, item);
                    }
                }

                await item.SetElementAsync(element, index, cancellationToken);

                if (this.map.Count >= 10)
                {
                    await Task.Delay(1, cancellationToken);
                }

                if (this.complete)
                {
                    this.OnComplete();
                }
            }

            public void SetComplete(
                int terminatingIndex)
            {
                lock (sink)
                {
                    if (this.complete)
                    {
                        return;
                    }

                    this.complete = true;
                    this.terminatingIndex = terminatingIndex;
                }
            }
            public bool ShouldComplete(
                int index)
            {
                lock (sink)
                {
                    return this.complete && index >= this.terminatingIndex;
                }
            }

            private void OnComplete()
            {
                if (this.map.Values.All(x => x.IsCompleted))
                {
                    this.yield.Break();
                }
            }

            private Task OnNextAsync(
                ParallelResult value,
                CancellationToken cancellationToken)
            {
                var result = value.ToList();
                this.map.Remove(value.CollectionIndex);

                return this.yield.ReturnAsync(result, cancellationToken);
            }

            private sealed class ParallelResult
            {
                private readonly IDictionary<int, TSource> elements = new SortedDictionary<int, TSource>();

                private readonly ParallelResultCollection parent;

                private readonly int collectionIndex;

                public ParallelResult(
                    int collectionIndex, 
                    ParallelResultCollection parent)
                {
                    this.collectionIndex = collectionIndex;
                    this.parent = parent;
                }

                public int CollectionIndex { get { return this.collectionIndex; } }

                public bool IsCompleted { get; private set; }

                public async Task SetElementAsync(
                    TSource element, 
                    int index, 
                    CancellationToken cancellationToken)
                {
                    this.elements.Add(index, element);
                    if (this.elements.Count == this.parent.count)
                    {
                        this.IsCompleted = true;
                        var result = this.elements.Values.ToList();

                        await this.parent.yield.ReturnAsync(result, cancellationToken);
                    }
                }

                public IList<TSource> ToList()
                {
                    return this.elements.Values.ToList();
                }
            }
        }
    }
}
