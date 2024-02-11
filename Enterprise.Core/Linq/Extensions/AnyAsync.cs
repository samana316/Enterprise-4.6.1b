using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        public static Task<bool> AnyAsync<TSource>(
            this IAsyncEnumerable<TSource> source)
        {
            return source.AnyAsync(CancellationToken.None);
        }

        public static async Task<bool> AnyAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            CancellationToken cancellationToken)
        {
            Check.NotNull(source, nameof(source));

            using (var iterator = source.GetAsyncEnumerator())
            {
                return await iterator.MoveNextAsync(cancellationToken);
            }
        }

        public static Task<bool> AnyAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, bool> predicate)
        {
            return source.AnyAsync(predicate, CancellationToken.None);
        }

        public static async Task<bool> AnyAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, bool> predicate,
            CancellationToken cancellationToken)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(predicate, nameof(predicate));

            using (var iterator = source.GetAsyncEnumerator())
            {
                while (await iterator.MoveNextAsync(cancellationToken))
                {
                    if (predicate(iterator.Current))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
