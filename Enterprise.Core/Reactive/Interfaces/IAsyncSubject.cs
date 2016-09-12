using System.Threading;
using System.Threading.Tasks;

namespace Enterprise.Core.Reactive
{
    public interface IAsyncSubject<T> : IAsyncObservable<T>
    {
        Task RunAsync(CancellationToken cancellationToken);
    }
}
