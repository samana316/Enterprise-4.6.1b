using System;
using System.Threading;

namespace Enterprise.Core.Reactive
{
    public interface IAsyncObservable<out T> : IObservable<T>
    {
        IAsyncSubscription SubscribeAsync(
            IAsyncObserver<T> observer, CancellationToken cancellationToken);
    }
}
