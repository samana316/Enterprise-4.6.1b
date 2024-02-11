using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        public static Task<long> LongCountAsync<TSource>(
            this IAsyncEnumerable<TSource> source)
        {
            return source.LongCountAsync(CancellationToken.None);
        }

        public static async Task<long> LongCountAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            CancellationToken cancellationToken)
        {
            Check.NotNull(source, nameof(source));

            int fastCount;
            if (source.TryFastCount(out fastCount))
            {
                return fastCount;
            }

            checked
            {
                var count = 0L;
                using (var iterator = source.GetAsyncEnumerator())
                {
                    while (await iterator.MoveNextAsync(cancellationToken))
                    {
                        count++;
                    }
                }

                return count;
            }
        }

        public static Task<long> LongCountAsync<TSource>(
           this IAsyncEnumerable<TSource> source,
           Func<TSource, bool> predicate)
        {
            return source.LongCountAsync(predicate, CancellationToken.None);
        }

        public static async Task<long> LongCountAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, bool> predicate,
            CancellationToken cancellationToken)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(predicate, nameof(predicate));

            checked
            {
                var count = 0L;
                await source.ForEachAsync((item) =>
                {
                    if (predicate(item))
                    {
                        count++;
                    }
                }, cancellationToken);

                return count;
            }
        }
    }
}
