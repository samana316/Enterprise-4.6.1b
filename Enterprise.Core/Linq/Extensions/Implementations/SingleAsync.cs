using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Resources;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        internal static async Task<TSource> SingleAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            bool throwOnEmpty,
            CancellationToken cancellationToken)
        {
            Check.NotNull(source, nameof(source));

            using (var iterator = source.GetEnumerator())
            {
                if (!await iterator.MoveNextAsync(cancellationToken))
                {
                    if (throwOnEmpty)
                    {
                        throw Error.EmptySequence();
                    }

                    return default(TSource);
                }

                var item = iterator.Current;

                if (await iterator.MoveNextAsync(cancellationToken))
                {
                    throw Error.MoreThanOneElement();
                }

                return item;
            }
        }

        internal static async Task<TSource> SingleAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, bool> predicate,
            bool throwOnEmpty,
            CancellationToken cancellationToken)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(predicate, nameof(predicate));

            var item = default(TSource);
            var found = false;

            await source.ForEachAsync(current => 
            {
                if (predicate(current))
                {
                    if (found)
                    {
                        throw Error.MoreThanOneMatch();
                    }

                    found = true;
                    item = current;
                }
            }, cancellationToken);

            if (!found && throwOnEmpty)
            {
                throw Error.NoMatch();
            }

            return item;
        }
    }
}
