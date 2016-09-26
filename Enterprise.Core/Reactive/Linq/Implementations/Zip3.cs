using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;

namespace Enterprise.Core.Reactive.Linq.Implementations
{
    internal sealed class Zip<TFirst, TSecond, TResult> : AsyncObservableBase<TResult>
    {
        private readonly IAsyncObservable<TFirst> first;

        private readonly IAsyncObservable<TSecond> second;

        private readonly IAsyncEnumerable<TSecond> secondE;

        private readonly Func<TFirst, TSecond, TResult> resultSelector;

        private Zip(
            IAsyncObservable<TFirst> first, 
            IAsyncObservable<TSecond> second, 
            IAsyncEnumerable<TSecond> secondE, 
            Func<TFirst, TSecond, TResult> resultSelector)
        {
            this.first = first;
            this.second = second;
            this.secondE = secondE;
            this.resultSelector = resultSelector;
        }

        public Zip(
            IAsyncObservable<TFirst> first,
            IObservable<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector)
        {
            this.first = first;
            this.second = second.AsAsyncObservable();
            this.resultSelector = resultSelector;
        }

        public Zip(
            IAsyncObservable<TFirst> first,
            IEnumerable<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector)
        {
            this.first = first;
            this.secondE = second.AsAsyncEnumerable();
            this.resultSelector = resultSelector;
        }

        public override AsyncIterator<TResult> Clone()
        {
            return new Zip<TFirst, TSecond, TResult>(
                this.first, this.second, this.secondE, this.resultSelector);
        }

        protected override Task ProduceAsync(
            IAsyncYield<TResult> yield, 
            CancellationToken cancellationToken)
        {
            if (this.secondE == null)
            {
                var producer = new ZipImpl(this);

                return producer.RunAsync(yield, cancellationToken);
            }

            return this.ZipImplEAsync(yield, cancellationToken);
        }

        private async Task ZipImplEAsync(
            IAsyncYield<TResult> yield,
            CancellationToken cancellationToken)
        {
            using (var iterator = this.secondE.GetAsyncEnumerator())
            {
                await this.first.ForEachAsync(async (item, cancellationToken2) =>
                {
                    if (await iterator.MoveNextAsync(cancellationToken2))
                    {
                        var result = this.resultSelector(item, iterator.Current);
                        await yield.ReturnAsync(result, cancellationToken2);
                    }
                    else
                    {
                        yield.Break();
                    }
                }, cancellationToken);
            }
        }

        private sealed class ZipImpl : IAsyncYieldBuilder<TResult>
        {
            private readonly Zip<TFirst, TSecond, TResult> parent;

            private ParallelResultCollection results;

            private int index1;

            private int index2;

            public ZipImpl(
                Zip<TFirst, TSecond, TResult> parent)
            {
                this.parent = parent;
            }

            public Task RunAsync(
                IAsyncYield<TResult> yield,
                CancellationToken cancellationToken)
            {
                this.results = new ParallelResultCollection(this.parent, yield);

                var task1 = this.RunFirstAsync(cancellationToken);
                var task2 = this.RunSecondAsync(cancellationToken);

                return Task.WhenAll(task1, task2);
            }

            private async Task RunFirstAsync(
                CancellationToken cancellationToken)
            {
                try
                {
                    await this.parent.first.ForEachAsync((item, cancellationToken2) =>
                    {
                        var cachedIndex = this.index1++;
                        return this.results.SetFirstAsync(item, cachedIndex, cancellationToken2);
                    }, cancellationToken);
                }
                finally
                {
                    this.results.SetComplete();
                }
            }

            private async Task RunSecondAsync(
               CancellationToken cancellationToken)
            {
                try
                {
                    await this.parent.second.ForEachAsync((item, cancellationToken2) =>
                    {
                        var cachedIndex = this.index2++;
                        return this.results.SetSecondAsync(item, cachedIndex, cancellationToken2);
                    }, cancellationToken);
                }
                finally
                {
                    this.results.SetComplete();
                }
            }
        }

        private sealed class ParallelResultCollection
        {
            private readonly object sink = new object();

            private readonly IDictionary<int, ParallelResult> map = new SortedDictionary<int, ParallelResult>();

            private readonly IAsyncYield<TResult> yield;

            private readonly Zip<TFirst, TSecond, TResult> parent;

            private bool complete;

            public ParallelResultCollection(
                Zip<TFirst, TSecond, TResult> parent, 
                IAsyncYield<TResult> yield)
            {
                this.parent = parent;
                this.yield = yield;
            }

            public async Task SetFirstAsync(
                TFirst first, 
                int index, 
                CancellationToken cancellationToken)
            {
                ParallelResult item;
                lock (sink)
                {
                    if (!this.map.TryGetValue(index, out item))
                    {
                        item = new ParallelResult(index, this.OnNextAsync);
                        this.map.Add(index, item);
                    }
                }

                await item.SetFirstAsync(first, cancellationToken);

                if (this.complete)
                {
                    this.OnComplete();
                }
            }

            public async Task SetSecondAsync(
                TSecond second,
                int index,
                CancellationToken cancellationToken)
            {
                ParallelResult item;
                lock (sink)
                {
                    if (!this.map.TryGetValue(index, out item))
                    {
                        item = new ParallelResult(index, this.OnNextAsync);
                        this.map.Add(index, item);
                    }
                }

                await item.SetSecondAsync(second, cancellationToken);

                if (this.complete)
                {
                    this.OnComplete();
                }
            }

            public void SetComplete()
            {
                this.complete = true;
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
                var result = this.parent.resultSelector(value.First, value.Second);
                this.map.Remove(value.Index);

                return this.yield.ReturnAsync(result, cancellationToken);
            }

            private sealed class ParallelResult
            {
                private readonly Func<ParallelResult, CancellationToken, Task> onNextAsync;

                private bool hasFirst;

                private bool hasSecond;

                public TFirst First { get; private set; }

                public TSecond Second { get; private set; }

                public int Index { get; private set; }

                public bool IsCompleted
                {
                    get { return this.hasFirst && this.hasSecond; }
                }

                public ParallelResult(
                    int index,
                    Func<ParallelResult, CancellationToken, Task> onNextAsync)
                {
                    this.Index = index;
                    this.onNextAsync = onNextAsync;
                }

                public async Task SetFirstAsync(
                    TFirst first,
                    CancellationToken cancellationToken)
                {
                    this.First = first;
                    this.hasFirst = true;

                    if (this.hasSecond)
                    {
                        await this.OnNextAsync(cancellationToken);
                    }
                }

                public async Task SetSecondAsync(
                    TSecond second,
                    CancellationToken cancellationToken)
                {
                    this.Second = second;
                    this.hasSecond = true;

                    if (this.hasFirst)
                    {
                        await this.OnNextAsync(cancellationToken);
                    }
                }

                private Task OnNextAsync(
                    CancellationToken cancellationToken)
                {
                    return this.onNextAsync(this, cancellationToken);
                }
            }
        }
    }
}
