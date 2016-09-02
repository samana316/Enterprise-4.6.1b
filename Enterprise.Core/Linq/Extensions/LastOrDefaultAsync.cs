using System;
using System.Threading;
using System.Threading.Tasks;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        public static Task<TSource> LastOrDefaultAsync<TSource>(
            this IAsyncEnumerable<TSource> source)
        {
            return source.LastOrDefaultAsync(CancellationToken.None);
        }

        public static Task<TSource> LastOrDefaultAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            CancellationToken cancellationToken)
        {
            return source.LastAsync(false, cancellationToken);
        }

        public static Task<TSource> LastOrDefaultAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, bool> predicate)
        {
            return source.LastOrDefaultAsync(predicate, CancellationToken.None);
        }

        public static Task<TSource> LastOrDefaultAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, bool> predicate,
            CancellationToken cancellationToken)
        {
            return source.LastAsync(predicate, false, cancellationToken);
        }
    }
}
