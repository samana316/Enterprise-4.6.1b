using System.Threading;

namespace Enterprise.Core.Reactive
{
    public interface IConnectableAsyncObservable<T> : IAsyncObservable<T>
    {
        IAsyncSubscription ConnectAsync(CancellationToken cancellationToken);
    }
}
