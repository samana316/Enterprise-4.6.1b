using System.Threading;
using System.Threading.Tasks;

namespace Enterprise.Core.Linq
{
    public interface IAsyncYield<TResult>
    {
        Task ReturnAsync(TResult value, CancellationToken cancellationToken);

        void Break();
    }
}
