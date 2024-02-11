using System;
using System.Threading;
using System.Threading.Tasks;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        public static Task<TSource> FirstAsync<TSource>(
            this IAsyncEnumerable<TSource> source)
        {
            return source.FirstAsync(CancellationToken.None);
        }

        public static Task<TSource> FirstAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            CancellationToken cancellationToken)
        {
            return source.FirstAsync(true, cancellationToken);
        }

        public static Task<TSource> FirstAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, bool> predicate)
        {
            return source.FirstAsync(predicate, CancellationToken.None);
        }

        public static Task<TSource> FirstAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, bool> predicate,
            CancellationToken cancellationToken)
        {
            return source.FirstAsync(predicate, true, cancellationToken);
        }
    }
}
