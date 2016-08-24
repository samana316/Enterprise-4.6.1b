using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Linq
{
    public static class AsyncEnumerator
    {
        public static Task<bool> MoveNextAsync(
           this IAsyncEnumerator enumerator)
        {
            Check.NotNull(enumerator, nameof(enumerator));

            return enumerator.MoveNextAsync(CancellationToken.None);
        }
    }
}
