using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        public static Task<bool> AllAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, bool> predicate)
        {
            return source.AllAsync(predicate, CancellationToken.None);
        }

        public static async Task<bool> AllAsync<TSource>(
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
                    if (!predicate(iterator.Current))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
