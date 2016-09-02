using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Resources;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        internal static async Task<TSource> LastAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            bool throwOnEmpty,
            CancellationToken cancellationToken)
        {
            Check.NotNull(source, nameof(source));

            var result = default(TSource);
            var count = 0;

            if (source.TryFastCount(out count))
            {
                if (count == 0)
                {
                    if (throwOnEmpty)
                    {
                        throw Error.EmptySequence();
                    }

                    return result;
                }
                
                if (source.TryFastIndexer(count - 1, out result))
                {
                    return result;
                }
            }

            using (var iterator = source.GetAsyncEnumerator())
            {
                if (!await iterator.MoveNextAsync(cancellationToken))
                {
                    if (throwOnEmpty)
                    {
                        throw Error.EmptySequence();
                    }

                    return result;
                }

                var last = iterator.Current;
                while (await iterator.MoveNextAsync(cancellationToken))
                {
                    last = iterator.Current;
                }

                return last;
            }
        }

        internal static async Task<TSource> LastAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, bool> predicate,
            bool throwOnEmpty,
            CancellationToken cancellationToken)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(predicate, nameof(predicate));

            var found = false;
            var last = default(TSource);

            await source.ForEachAsync(item => 
            {
                if (predicate(item))
                {
                    found = true;
                    last = item;
                }
            }, cancellationToken);

            if (!found && throwOnEmpty)
            {
                throw Error.NoMatch();
            }

            return last;
        }
    }
}
