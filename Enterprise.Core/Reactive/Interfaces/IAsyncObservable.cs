using System;
using System.Threading;

namespace Enterprise.Core.Reactive
{
    public interface IAsyncObservable<T> : IObservable<T>
    {
        IAsyncSubscription SubscribeAsync(
            IAsyncObserver<T> observer, CancellationToken cancellationToken);
    }
}
