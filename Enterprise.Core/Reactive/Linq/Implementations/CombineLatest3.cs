using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;

namespace Enterprise.Core.Reactive.Linq.Implementations
{
    internal sealed class CombineLatest<TFirst, TSecond, TResult> : AsyncObservableBase<TResult>
    {
        private readonly IAsyncObservable<TFirst> first;

        private readonly IAsyncObservable<TSecond> second;

        private readonly Func<TFirst, TSecond, TResult> resultSelector;

        public CombineLatest(
            IAsyncObservable<TFirst> first,
            IObservable<TSecond> second, 
            Func<TFirst, TSecond, TResult> resultSelector)
        {
            this.first = first;
            this.second = second.AsAsyncObservable();
            this.resultSelector = resultSelector;
        }

        public override AsyncIterator<TResult> Clone()
        {
            return new CombineLatest<TFirst, TSecond, TResult>(
                this.first, this.second, this.resultSelector);
        }

        protected override Task ProduceAsync(
            IAsyncYield<TResult> yield, 
            CancellationToken cancellationToken)
        {
            var producer = new CombineLatestImpl(this);

            return producer.RunAsync(yield, cancellationToken);
        }

        private sealed class CombineLatestImpl : IAsyncYieldBuilder<TResult>
        {
            private readonly CombineLatest<TFirst, TSecond, TResult> parent;

            private IAsyncYield<TResult> yield;

            public CombineLatestImpl(
                CombineLatest<TFirst, TSecond, TResult> parent)
            {
                this.parent = parent;
            }

            public Task RunAsync(
                IAsyncYield<TResult> yield, 
                CancellationToken cancellationToken)
            {
                this.yield = yield;

                var f = new FirstImpl(this);
                var s = new SecondImpl(this);
                f.Other = s;
                s.Other = f;

                var task1 = this.parent.first.ForEachAsync(f.ReturnAsync, cancellationToken);
                var task2 = this.parent.second.ForEachAsync(s.ReturnAsync, cancellationToken);

                return Task.WhenAll(task1, task2);
            }

            private sealed class FirstImpl : IAsyncYield<TFirst>
            {
                private readonly CombineLatestImpl parent;

                public FirstImpl(
                    CombineLatestImpl parent)
                {
                    this.parent = parent;
                }

                public bool HasValue { get; private set; }

                public TFirst Value { get; private set; }

                public bool Done { get; private set; }

                public SecondImpl Other { get; set; }

                public void Break()
                {
                    this.Done = true;
                    this.parent.yield.Break();
                }

                public async Task ReturnAsync(
                    TFirst value,
                    CancellationToken cancellationToken)
                {
                    this.HasValue = true;
                    this.Value = value;

                    if (this.Other.HasValue)
                    {
                        var result = this.parent.parent.resultSelector(value, this.Other.Value);

                        await this.parent.yield.ReturnAsync(result, cancellationToken);
                    }
                    else if (this.Other.Done)
                    {
                        this.Break();
                    }
                }
            }

            private sealed class SecondImpl : IAsyncYield<TSecond>
            {
                private readonly CombineLatestImpl parent;

                public SecondImpl(
                    CombineLatestImpl parent)
                {
                    this.parent = parent;
                }

                public bool HasValue { get; private set; }

                public TSecond Value { get; private set; }

                public bool Done { get; private set; }

                public FirstImpl Other { get; set; }

                public void Break()
                {
                    this.Done = true;
                    this.parent.yield.Break();
                }

                public async Task ReturnAsync(
                    TSecond value,
                    CancellationToken cancellationToken)
                {
                    this.HasValue = true;
                    this.Value = value;

                    if (this.Other.HasValue)
                    {
                        var result = this.parent.parent.resultSelector(this.Other.Value, value);

                        await this.parent.yield.ReturnAsync(result, cancellationToken);
                    }
                    else if (this.Other.Done)
                    {
                        this.Break();
                    }
                }
            }
        }
    }
}
