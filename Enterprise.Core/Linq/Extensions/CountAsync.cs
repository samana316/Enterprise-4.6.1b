using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        public static Task<int> CountAsync<TSource>(
            this IAsyncEnumerable<TSource> source)
        {
            return source.CountAsync(CancellationToken.None);
        }

        public static async Task<int> CountAsync<TSource>(
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
                var count = 0;
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

        public static Task<int> CountAsync<TSource>(
           this IAsyncEnumerable<TSource> source,
           Func<TSource, bool> predicate)
        {
            return source.CountAsync(predicate, CancellationToken.None);
        }

        public static async Task<int> CountAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, bool> predicate,
            CancellationToken cancellationToken)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(predicate, nameof(predicate));

            checked
            {
                var count = 0;
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
