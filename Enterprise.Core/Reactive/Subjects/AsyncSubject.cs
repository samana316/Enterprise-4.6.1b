using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Common;
using Enterprise.Core.Common.Runtime.CompilerServices;
using Enterprise.Core.Reactive.Linq;

namespace Enterprise.Core.Reactive.Subjects
{
    public sealed class AsyncSubject<T> : AsyncSubjectBase<T>
    {
        private readonly object sink = new object();

        private readonly CompositeAsyncObserver<T> observers = new CompositeAsyncObserver<T>();

        private readonly ICollection<IDisposable> subscriptions = new List<IDisposable>();

        private bool isCompleted;

        private bool isDisposed;

        public override bool HasObservers
        {
            get
            {
                return this.observers.Any();
            }
        }

        public override bool IsDisposed
        {
            get
            {
                return this.isDisposed;
            }
        }

        public override void OnCompleted()
        {
            if (this.isCompleted)
            {
                return;
            }

            this.observers.OnCompleted();
            this.isCompleted = true;
        }

        public override void OnError(
            Exception error)
        {
            if (this.isCompleted)
            {
                return;
            }

            this.observers.OnError(error);
            this.OnCompleted();
        }

        public override void OnNext(
            T value)
        {
            if (this.isCompleted)
            {
                return;
            }

            this.observers.OnNext(value);
        }

        public override async Task OnNextAsync(
            T value, 
            CancellationToken cancellationToken)
        {
            if (this.isCompleted)
            {
                return;
            }

            await this.observers.OnNextAsync(value, cancellationToken);
        }

        protected override void Dispose(
            bool disposing)
        {
            this.isCompleted = true;

            if (this.isDisposed)
            {
                return;
            }

            lock (sink)
            {
                if (disposing)
                {
                    foreach (var subscription in this.subscriptions)
                    {
                        if (ReferenceEquals(subscription, null))
                        {
                            continue;
                        }

                        subscription.Dispose();
                    }

                    this.observers.Clear();
                }

                base.Dispose(disposing);
                this.isDisposed = true;
            }
        }

        public override IDisposable Subscribe(
            IObserver<T> observer)
        {
            var adapter = observer.AsAsyncObserver();

            return this.InternalSubscribe(adapter);
        }

        public override IAsyncSubscription SubscribeAsync(
            IAsyncObserver<T> observer,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.InternalSubscribe(observer);
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

        private sealed class Subscriber
        {
            private readonly AsyncSubject<T> parent;

            private readonly IAsyncObserver<T> observer;

            public Subscriber(
                AsyncSubject<T> parent,
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

            private readonly AsyncSubject<T> parent;

            private readonly IAsyncObserver<T> observer;

            public Unsubscriber(
                AsyncSubject<T> parent,
                IAsyncObserver<T> observer)
            {
                this.parent = parent;
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
