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
                return this.SelectManyImplAsync(yield, cancellationToken);
            }

            return this.SelectManyImplEAsync(yield, cancellationToken);
        }

        private async Task SelectManyImplEAsync(
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

        private Task SelectManyImplAsync(
            IAsyncYield<TResult> yield,
            CancellationToken cancellationToken)
        {
            var results = new ParallelResultCollection(this, yield);
            var index1 = -1;
            var index2 = -1;

            var task1 = this.first.ForEachAsync(async (item, cancellationToken2) =>
            {
                index1++;
                await results.SetFirstAsync(item, index1, cancellationToken2);
            }, cancellationToken).ContinueWith(t => results.SetComplete());

            var task2 = this.second.ForEachAsync(async (item, cancellationToken2) =>
            {
                index2++;
                await results.SetSecondAsync(item, index2, cancellationToken2);
            }, cancellationToken).ContinueWith(t => results.SetComplete());

            return Task.WhenAll(task1, task2);
        }

        private sealed class ParallelResultCollection
        {
            private readonly IDictionary<int, ParallelResult> map = new Dictionary<int, ParallelResult>();

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
                if (!this.map.TryGetValue(index, out item))
                {
                    item = new ParallelResult(this.OnNextAsync);
                    this.map.Add(index, item);
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
                if (!this.map.TryGetValue(index, out item))
                {
                    item = new ParallelResult(this.OnNextAsync);
                    this.map.Add(index, item);
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
                TFirst first,
                TSecond second,
                CancellationToken cancellationToken)
            {
                var result = this.parent.resultSelector(first, second);

                return this.yield.ReturnAsync(result, cancellationToken);
            }

            private sealed class ParallelResult
            {
                private readonly Func<TFirst, TSecond, CancellationToken, Task> onNextAsync;

                private bool hasFirst;

                private bool hasSecond;

                private TFirst first;

                private TSecond second;

                public bool IsCompleted
                {
                    get { return this.hasFirst && this.hasSecond; }
                }

                public ParallelResult(
                    Func<TFirst, TSecond, CancellationToken, Task> onNextAsync)
                {
                    this.onNextAsync = onNextAsync;
                }

                public async Task SetFirstAsync(
                    TFirst first,
                    CancellationToken cancellationToken)
                {
                    this.first = first;
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
                    this.second = second;
                    this.hasSecond = true;

                    if (this.hasFirst)
                    {
                        await this.OnNextAsync(cancellationToken);
                    }
                }

                private Task OnNextAsync(
                    CancellationToken cancellationToken)
                {
                    return this.onNextAsync(this.first, this.second, cancellationToken);
                }
            }
        }
    }
}
