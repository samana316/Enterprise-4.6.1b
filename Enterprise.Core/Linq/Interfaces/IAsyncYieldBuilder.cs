using System.Threading;
using System.Threading.Tasks;

namespace Enterprise.Core.Linq
{
    public interface IAsyncYieldBuilder<T>
    {
        Task RunAsync(IAsyncYield<T> yield, CancellationToken cancellationToken);
    }
}
