using System;
using System.Threading;
using System.Threading.Tasks;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        public static Task<TSource> LastAsync<TSource>(
            this IAsyncEnumerable<TSource> source)
        {
            return source.LastAsync(CancellationToken.None);
        }

        public static Task<TSource> LastAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            CancellationToken cancellationToken)
        {
            return source.LastAsync(true, cancellationToken);
        }

        public static Task<TSource> LastAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, bool> predicate)
        {
            return source.LastAsync(predicate, CancellationToken.None);
        }

        public static Task<TSource> LastAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, bool> predicate,
            CancellationToken cancellationToken)
        {
            return source.LastAsync(predicate, true, cancellationToken);
        }
    }
}
