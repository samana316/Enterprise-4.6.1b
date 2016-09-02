using System;
using System.Threading;
using System.Threading.Tasks;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        public static Task<TSource> SingleOrDefaultAsync<TSource>(
            this IAsyncEnumerable<TSource> source)
        {
            return source.SingleOrDefaultAsync(CancellationToken.None);
        }

        public static Task<TSource> SingleOrDefaultAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            CancellationToken cancellationToken)
        {
            return source.SingleAsync(false, cancellationToken);
        }

        public static Task<TSource> SingleOrDefaultAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, bool> predicate)
        {
            return source.SingleOrDefaultAsync(predicate, CancellationToken.None);
        }

        public static Task<TSource> SingleOrDefaultAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, bool> predicate,
            CancellationToken cancellationToken)
        {
            return source.SingleAsync(predicate, false, cancellationToken);
        }
    }
}
