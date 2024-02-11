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
    public sealed class ReplayAsyncSubject<T> : AsyncSubjectBase<T>
    {
        private readonly object sink = new object();

        private readonly CompositeAsyncObserver<T> observers = new CompositeAsyncObserver<T>();

        private readonly ICollection<IDisposable> subscriptions = new List<IDisposable>();

        private readonly Queue<T> buffer = new Queue<T>();

        private readonly int? bufferSize;

        private bool isCompleted;

        private bool isDisposed;

        public ReplayAsyncSubject()
        {
        }

        public ReplayAsyncSubject(
            int bufferSize)
        {
            this.bufferSize = bufferSize;
        }

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
            this.OnNextAsync(value).Wait();
        }

        public override async Task OnNextAsync(
            T value,
            CancellationToken cancellationToken)
        {
            if (this.isCompleted)
            {
                return;
            }

            this.buffer.Enqueue(value);

            if (this.bufferSize.HasValue && this.buffer.Count > this.bufferSize.Value)
            {
                this.buffer.Dequeue();
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
            var subscription = this.SubscribeAsync(observer.AsAsyncObserver());
            subscription.GetAwaiter().GetResult();

            return subscription;
        }

        public override IAsyncSubscription SubscribeAsync(
            IAsyncObserver<T> observer,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            lock (sink)
            {
                if (!this.observers.Contains(observer))
                {
                    this.observers.Add(observer);
                }

                var subscription = new Unsubscriber(this, observer, cancellationToken);

                this.subscriptions.Add(subscription);

                return subscription;
            }
        }

        internal override async Task SubscribeCoreAsync(
            IAsyncObserver<T> observer, 
            CancellationToken cancellationToken)
        {
            foreach (var item in this.buffer)
            {
                await observer.OnNextAsync(item, cancellationToken);
            }
        }

        private sealed class Unsubscriber : DisposableBase, IAsyncSubscription
        {
            private readonly object sink = new object();

            private readonly ReplayAsyncSubject<T> parent;

            private readonly IAsyncObserver<T> observer;

            private readonly CancellationToken cancellationToken;

            public Unsubscriber(
                ReplayAsyncSubject<T> parent,
                IAsyncObserver<T> observer, 
                CancellationToken cancellationToken)
            {
                this.parent = parent;
                this.observer = observer;
                this.cancellationToken = cancellationToken;
            }

            public IAwaiter GetAwaiter()
            {
                return this.ReplayAsync().FromTask().GetAwaiter();
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

            private Task ReplayAsync()
            {
                return this.parent.SubscribeCoreAsync(this.observer, this.cancellationToken);
            }
        }
    }
}
