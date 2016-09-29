using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Core.Utilities;
using Enterprise.Core.Reactive.Linq;

namespace Enterprise.Core.Reactive
{
    internal abstract class AsyncObservableBase<T> : AsyncEnumerableBase<T>, IAsyncObservable<T>
    {
        public IDisposable Subscribe(
            IObserver<T> observer)
        {
            Check.NotNull(observer, nameof(observer));
            var adapter = observer.AsAsyncObserver();

            return this.SubscribeAsync(adapter);
        }

        public IAsyncSubscription SubscribeAsync(
            IAsyncObserver<T> observer, 
            CancellationToken cancellationToken)
        {
            Check.NotNull(observer, nameof(observer));
            cancellationToken.ThrowIfCancellationRequested();

            var task = this.SubscribeSafeAsync(observer, cancellationToken);

            return new AsyncSubscription(task, cancellationToken);
        }

        protected override sealed Task EnumerateAsync(
            IAsyncYield<T> yield, 
            CancellationToken cancellationToken)
        {
            return this.ProduceAsync(yield, cancellationToken);
        }

        protected abstract Task ProduceAsync(
            IAsyncYield<T> yield, CancellationToken cancellationToken);

        internal async Task SubscribeCoreAsync(
            IAsyncObserver<T> observer,
            CancellationToken cancellationToken)
        {
            try
            {
                await this.UnsafeSubscribeCoreAsync(observer, cancellationToken);
            }
            catch (AsyncObservableCanceledException)
            {
            }
        }

        internal async Task UnsafeSubscribeCoreAsync(
            IAsyncObserver<T> observer,
            CancellationToken cancellationToken)
        {
            var producer = new Producer(observer);

            await this.ProduceAsync(producer, cancellationToken);
        }

        internal async Task SubscribeSafeAsync(
            IAsyncObserver<T> observer,
            CancellationToken cancellationToken)
        {
            try
            {
                await this.SubscribeCoreAsync(observer, cancellationToken);
            }
            catch (Exception exception)
            {
                observer.OnError(exception);
            }
            finally
            {
                observer.OnCompleted();
            }
        }

        private sealed class Producer : IAsyncYield<T>
        {
            private readonly IAsyncObserver<T> observer;

            public Producer(
                IAsyncObserver<T> observer)
            {
                this.observer = observer;
            }

            public void Break()
            {
                this.observer.OnCompleted();

                throw new AsyncObservableCanceledException();
            }

            public Task ReturnAsync(
                T value,
                CancellationToken cancellationToken)
            {
                return this.observer.OnNextAsync(value, cancellationToken);
            }
        }
    }
}
