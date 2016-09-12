using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Common;
using Enterprise.Core.Common.Runtime.CompilerServices;
using Enterprise.Core.Reactive.Linq;

namespace Enterprise.Core.Reactive
{
    internal abstract class AsyncSubjectBase<T> : AsyncObservableBase<T>, IAsyncSubject<T>
    {
        private readonly object sink = new object();

        private readonly CompositeAsyncObserver<T> observers = new CompositeAsyncObserver<T>();

        private readonly ICollection<IDisposable> subscriptions = new List<IDisposable>();

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

        public Task RunAsync(
            CancellationToken cancellationToken)
        {
            return this.SubscribeCoreAsync(this.observers, cancellationToken);
        }

        private sealed class Subscriber
        {
            private readonly AsyncSubjectBase<T> parent;

            private readonly IAsyncObserver<T> observer;

            public Subscriber(
                AsyncSubjectBase<T> parent, 
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

            private readonly AsyncSubjectBase<T> parent;

            private readonly IAsyncObserver<T> observer;

            public Unsubscriber(
                AsyncSubjectBase<T> observable,
                IAsyncObserver<T> observer)
            {
                this.parent = observable;
                this.observer = observer;
            }

            public IAwaiter GetAwaiter()
            {
                return new CompleteAwaiter();
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

        private sealed class CompleteAwaiter : IAwaiter
        {
            public bool IsCompleted
            { 
                get { return true; }
            }

            public void GetResult()
            {
            }

            public void OnCompleted(
                Action continuation)
            {
                continuation();
            }

            public void UnsafeOnCompleted(
                Action continuation)
            {
                continuation();
            }
        }
    }
}
