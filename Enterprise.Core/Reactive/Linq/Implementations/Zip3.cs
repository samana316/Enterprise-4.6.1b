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
                var producer = new Zip_(this);

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

        private sealed class Zip_ : IAsyncYieldBuilder<TResult>
        {
            private readonly Zip<TFirst, TSecond, TResult> parent;

            private object gate;

            private IAsyncYield<TResult> yield;

            public Zip_(
                Zip<TFirst, TSecond, TResult> parent)
            {
                this.parent = parent;
            }

            public Task RunAsync(
                IAsyncYield<TResult> yield,
                CancellationToken cancellationToken)
            {
                this.gate = new object();
                this.yield = yield;

                var f = new FirstImpl(this);
                var s = new SecondImpl(this);
                f.Other = s;
                s.Other = f;

                var task1 = this.parent.first.ForEachAsync(f, cancellationToken);
                var task2 = this.parent.second.ForEachAsync(s, cancellationToken);

                return Task.WhenAll(task1, task2);
            }

            private sealed class FirstImpl : IAsyncYield<TFirst>
            {
                private readonly Zip_ parent;

                public FirstImpl(
                    Zip_ parent)
                {
                    this.parent = parent;
                    this.Queue = new Queue<TFirst>();
                }

                public Queue<TFirst> Queue { get; private set; }

                public bool Done { get; private set; }

                public SecondImpl Other { get; set; }

                public void Break()
                {
                    lock (this.parent.gate)
                    {
                        this.Done = true;
                        if (this.Other.Done)
                        {
                            this.parent.yield.Break();
                        }
                    }
                }

                public Task ReturnAsync(
                    TFirst value,
                    CancellationToken cancellationToken)
                {
                    var task = Task.CompletedTask;

                    lock (this.parent.gate)
                    {
                        if (this.Other.Queue.Any())
                        {
                            var other = this.Other.Queue.Dequeue();
                            var result = this.parent.parent.resultSelector(value, other);

                            task = this.parent.yield.ReturnAsync(result, cancellationToken);
                        }
                        else if (this.Other.Done)
                        {
                            this.Break();
                        }
                        else
                        {
                            this.Queue.Enqueue(value);
                        }
                    }

                    return task;
                }
            }

            private sealed class SecondImpl : IAsyncYield<TSecond>
            {
                private readonly Zip_ parent;

                public SecondImpl(
                    Zip_ parent)
                {
                    this.parent = parent;
                    this.Queue = new Queue<TSecond>();
                }

                public Queue<TSecond> Queue { get; private set; }

                public bool Done { get; private set; }

                public FirstImpl Other { get; set; }

                public void Break()
                {
                    lock (this.parent.gate)
                    {
                        this.Done = true;
                        if (this.Other.Done)
                        {
                            this.parent.yield.Break();
                        }
                    }
                }

                public Task ReturnAsync(
                    TSecond value,
                    CancellationToken cancellationToken)
                {
                    var task = Task.CompletedTask;

                    lock (this.parent.gate)
                    {
                        if (this.Other.Queue.Any())
                        {
                            var other = this.Other.Queue.Dequeue();
                            var result = this.parent.parent.resultSelector(other, value);

                            task = this.parent.yield.ReturnAsync(result, cancellationToken);
                        }
                        else if (this.Other.Done)
                        {
                            this.Break();
                        }
                        else
                        {
                            this.Queue.Enqueue(value);
                        }
                    }

                    return task;
                }
            }
        }

        private sealed class ZipImpl : IAsyncYieldBuilder<TResult>
        {
            private readonly Zip<TFirst, TSecond, TResult> parent;

            private IAsyncYield<TResult> yield;

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
                this.yield = yield;
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
                        if (this.results.ShouldComplete(this.index1))
                        {
                            this.yield.Break();
                        }

                        var cachedIndex = this.index1++;
                        return this.results.SetFirstAsync(item, cachedIndex, cancellationToken2);
                    }, cancellationToken);
                }
                finally
                {
                    this.results.SetComplete(this.index1);
                }
            }

            private async Task RunSecondAsync(
               CancellationToken cancellationToken)
            {
                try
                {
                    await this.parent.second.ForEachAsync((item, cancellationToken2) =>
                    {
                        if (this.results.ShouldComplete(this.index2))
                        {
                            this.yield.Break();
                        }

                        var cachedIndex = this.index2++;
                        return this.results.SetSecondAsync(item, cachedIndex, cancellationToken2);
                    }, cancellationToken);
                }
                finally
                {
                    this.results.SetComplete(this.index2);
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

            private int terminatingIndex;

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
                var item = this.GetOrCreateResultContainer(index);
                await item.SetFirstAsync(first, cancellationToken);

                await this.CheckCompleteAsync(cancellationToken);
            }

            public async Task SetSecondAsync(
                TSecond second,
                int index,
                CancellationToken cancellationToken)
            {
                var item = this.GetOrCreateResultContainer(index);
                await item.SetSecondAsync(second, cancellationToken);

                await this.CheckCompleteAsync(cancellationToken);
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

            private async Task CheckCompleteAsync(
                CancellationToken cancellationToken)
            {
                if (this.map.Count >= 10)
                {
                    await Task.Delay(1, cancellationToken);
                }

                if (this.complete)
                {
                    this.OnComplete();
                }
            }

            private ParallelResult GetOrCreateResultContainer(
                int index)
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

                return item;
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
                private readonly object sink = new object();

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

                public Task SetFirstAsync(
                    TFirst first,
                    CancellationToken cancellationToken)
                {
                    lock (sink)
                    {
                        this.First = first;
                        this.hasFirst = true;

                        if (this.hasSecond)
                        {
                            return this.OnNextAsync(cancellationToken);
                        }

                        return Task.CompletedTask;
                    }
                }

                public Task SetSecondAsync(
                    TSecond second,
                    CancellationToken cancellationToken)
                {
                    lock (sink)
                    {
                        this.Second = second;
                        this.hasSecond = true;

                        if (this.hasFirst)
                        {
                            return this.OnNextAsync(cancellationToken);
                        }

                        return Task.CompletedTask;
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
