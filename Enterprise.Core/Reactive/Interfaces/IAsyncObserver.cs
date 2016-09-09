using System;
using System.Threading;
using System.Threading.Tasks;

namespace Enterprise.Core.Reactive
{
    public interface IAsyncObserver<T> : IObserver<T>
    {
        Task OnNextAsync(T value, CancellationToken cancellationToken);
    }
}
