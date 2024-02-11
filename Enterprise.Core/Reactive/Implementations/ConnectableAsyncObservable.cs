using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Common;
using Enterprise.Core.Common.Runtime.CompilerServices;
using Enterprise.Core.Linq;
using Enterprise.Core.Reactive.Linq;

namespace Enterprise.Core.Reactive
{
    internal sealed class ConnectableAsyncObservable<T> : 
        AsyncObservableBase<T>, 
        IConnectableAsyncObservable<T>
    {
        private readonly object sink = new object();

        private readonly CompositeAsyncObserver<T> observers = new CompositeAsyncObserver<T>();

        private readonly ICollection<IDisposable> subscriptions = new List<IDisposable>();

        private readonly IAsyncObservable<T> source;

        public ConnectableAsyncObservable(
            IAsyncObservable<T> source)
        {
            this.source = source;
        }

        public override AsyncIterator<T> Clone()
        {
            return new ConnectableAsyncObservable<T>(this.source);
        }

        protected override Task ProduceAsync(
            IAsyncYield<T> yield,
            CancellationToken cancellationToken)
        {
            return this.source.ForEachAsync((item, cancellationToken2) =>
            {
                return yield.ReturnAsync(item, cancellationToken2);
            }, cancellationToken);
        }

        IDisposable IObservable<T>.Subscribe(
            IObserver<T> observer)
        {
            var adapter = observer.AsAsyncObserver();

            return this.InternalSubscribe(adapter);
        }

        IAsyncSubscription IAsyncObservable<T>.SubscribeAsync(
            IAsyncObserver<T> observer,
            CancellationToken cancellationToken)
        {
            var task = this.InternalSubscribeAsync(observer, cancellationToken);

            return new AsyncSubscription(task, cancellationToken);
        }

        private Unsubscriber InternalSubscribe(
            IAsyncObserver<T> observer)
        {
            lock (sink)
            {
                if (!this.observers.Contains(observer))
                {
                    this.observers.Add(observer);
                }

                var subscription = new Unsubscriber(this, observer);

                this.subscriptions.Add(subscription);

                return subscription;
            }
        }

        private Task<Unsubscriber> InternalSubscribeAsync(
            IAsyncObserver<T> observer,
            CancellationToken cancellationToken)
        {
            var subscriber = new Subscriber(this, observer);

            Func<Unsubscriber> function = subscriber.Subscribe;
            var task = Task.Run(function, cancellationToken);

            while (!cancellationToken.IsCancellationRequested)
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    break;
                }

                cancellationToken.ThrowIfCancellationRequested();
            }

            return task;
        }

        public IAsyncSubscription ConnectAsync(
            CancellationToken cancellationToken)
        {
            var task = this.SubscribeCoreAsync(this.observers, cancellationToken);

            return new AsyncSubscription(task, cancellationToken);
        }

        private sealed class Subscriber
        {
            private readonly ConnectableAsyncObservable<T> parent;

            private readonly IAsyncObserver<T> observer;

            public Subscriber(
                ConnectableAsyncObservable<T> parent,
                IAsyncObserver<T> observer)
            {
                this.parent = parent;
                this.observer = observer;
            }

            public Unsubscriber Subscribe()
            {
                return this.parent.InternalSubscribe(this.observer);
            }
        }

        private sealed class Unsubscriber : DisposableBase, IAsyncSubscription
        {
            private readonly object sink = new object();

            private readonly ConnectableAsyncObservable<T> parent;

            private readonly IAsyncObserver<T> observer;

            public Unsubscriber(
                ConnectableAsyncObservable<T> observable,
                IAsyncObserver<T> observer)
            {
                this.parent = observable;
                this.observer = observer;
            }

            public IAwaiter GetAwaiter()
            {
                return Awaitable.FromCompletedTask().GetAwaiter();
            }

            protected override sealed void Dispose(
                bool disposing)
            {
                lock (sink)
                {
                    if (disposing)
                    {
                        if (this.observer != null &&
                            this.parent.observers.Contains(this.observer))
                        {
                            this.parent.observers.Remove(this.observer);
                        }
                    }

                    base.Dispose(disposing);
                }
            }
        }
    }
}
