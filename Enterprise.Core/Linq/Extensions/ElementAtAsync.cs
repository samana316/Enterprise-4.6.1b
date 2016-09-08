using System.Threading;
using System.Threading.Tasks;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        public static Task<TSource> ElementAtAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            int index)
        {
            return source.ElementAtAsync(index, CancellationToken.None);
        }

        public static Task<TSource> ElementAtAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            int index,
            CancellationToken cancellationToken)
        {
            return source.ElementAtAsync(index, true, cancellationToken);
        }
    }
}
