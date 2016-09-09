using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;

namespace Enterprise.Core.Reactive.Linq.Implementations
{
    internal sealed class AsAsyncObservable<TSource> : AsyncObservableBase<TSource>
    {
        private readonly IObservable<TSource> source;

        public AsAsyncObservable(
            IObservable<TSource> source)
        {
            this.source = source;
        }

        public override AsyncIterator<TSource> Clone()
        {
            return new AsAsyncObservable<TSource>(this.source);
        }

        protected override Task ProduceAsync(
            IAsyncYield<TSource> yield, 
            CancellationToken cancellationToken)
        {
            var observer = new AsAsyncObserver(yield, cancellationToken);

            var subscription = this.source.Subscribe(observer);

            return Task.WhenAll(observer.Tasks);
        }

        private sealed class AsAsyncObserver : AsyncObserverBase<TSource>
        {
            private readonly IAsyncYield<TSource> yield;

            private readonly CancellationToken cancellationToken;

            public AsAsyncObserver(
                IAsyncYield<TSource> yield,
                CancellationToken cancellationToken)
            {
                this.yield = yield;
                this.cancellationToken = cancellationToken;
                this.Tasks = new List<Task>();
            }

            public IList<Task> Tasks { get; private set; }

            public override void OnCompleted()
            {
                this.yield.Break();
            }

            public override void OnNext(
                TSource value)
            {
                var task = this.yield.ReturnAsync(value, this.cancellationToken);

                this.Tasks.Add(task);
            }

            public override Task OnNextAsync(
                TSource value, 
                CancellationToken cancellationToken)
            {
                return this.yield.ReturnAsync(value, cancellationToken);
            }
        }
    }
}
