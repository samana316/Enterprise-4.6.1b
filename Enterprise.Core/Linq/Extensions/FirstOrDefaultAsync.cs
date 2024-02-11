using System;
using System.Threading;
using System.Threading.Tasks;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        public static Task<TSource> FirstOrDefaultAsync<TSource>(
            this IAsyncEnumerable<TSource> source)
        {
            return source.FirstOrDefaultAsync(CancellationToken.None);
        }

        public static Task<TSource> FirstOrDefaultAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            CancellationToken cancellationToken)
        {
            return source.FirstAsync(false, cancellationToken);
        }

        public static Task<TSource> FirstOrDefaultAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, bool> predicate)
        {
            return source.FirstOrDefaultAsync(predicate, CancellationToken.None);
        }

        public static Task<TSource> FirstOrDefaultAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, bool> predicate,
            CancellationToken cancellationToken)
        {
            return source.FirstAsync(predicate, false, cancellationToken);
        }
    }
}
