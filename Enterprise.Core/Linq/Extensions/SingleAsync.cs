using System;
using System.Threading;
using System.Threading.Tasks;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        public static Task<TSource> SingleAsync<TSource>(
            this IAsyncEnumerable<TSource> source)
        {
            return source.SingleAsync(CancellationToken.None);
        }

        public static Task<TSource> SingleAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            CancellationToken cancellationToken)
        {
            return source.SingleAsync(true, cancellationToken);
        }

        public static Task<TSource> SingleAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, bool> predicate)
        {
            return source.SingleAsync(predicate, CancellationToken.None);
        }

        public static Task<TSource> SingleAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, bool> predicate,
            CancellationToken cancellationToken)
        {
            return source.SingleAsync(predicate, true, cancellationToken);
        }
    }
}
