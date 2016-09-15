using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Common;

namespace Enterprise.Core.Reactive.Subjects
{
    public abstract class AsyncSubjectBase<T> : DisposableBase, IAsyncSubject<T>
    {
        public abstract bool HasObservers { get; }

        public abstract bool IsDisposed { get; }

        public abstract void OnCompleted();

        public abstract void OnError(Exception error);

        public abstract void OnNext(T value);

        public abstract Task OnNextAsync(T value, CancellationToken cancellationToken);

        public abstract IDisposable Subscribe(IObserver<T> observer);

        public abstract IAsyncSubscription SubscribeAsync(IAsyncObserver<T> observer, CancellationToken cancellationToken);

        internal virtual Task SubscribeCoreAsync(
            IAsyncObserver<T> observer,
            CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }
    }
}
