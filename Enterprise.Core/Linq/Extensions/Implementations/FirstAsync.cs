using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Resources;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        internal static async Task<TSource> FirstAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            bool throwOnEmpty,
            CancellationToken cancellationToken)
        {
            Check.NotNull(source, nameof(source));

            using (var iterator = source.GetAsyncEnumerator())
            {
                if (await iterator.MoveNextAsync(cancellationToken))
                {
                    return iterator.Current;
                }
            }

            if (throwOnEmpty)
            {
                throw Error.EmptySequence();
            }

            return default(TSource);
        }

        internal static async Task<TSource> FirstAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, bool> predicate,
            bool throwOnEmpty,
            CancellationToken cancellationToken)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(predicate, nameof(predicate));

            using (var iterator = source.GetAsyncEnumerator())
            {
                while (await iterator.MoveNextAsync(cancellationToken))
                {
                    var item = iterator.Current;
                    if (predicate(item))
                    {
                        return item;
                    }
                }
            }

            if (throwOnEmpty)
            {
                throw Error.NoMatch();
            }

            return default(TSource);
        }
    }
}
