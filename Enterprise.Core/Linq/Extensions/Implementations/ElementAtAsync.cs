using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Resources;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        private static async Task<TSource> ElementAtAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            int index,
            bool throwOnEmpty,
            CancellationToken cancellationToken)
        {
            Check.NotNull(source, nameof(source));

            var element = default(TSource);
            if (source.TryFastIndexer(index, out element))
            {
                return element;
            }

            if (index < 0)
            {
                return ReturnOrThrow<TSource>(index, throwOnEmpty);
            }

            using (var iterator = source.GetAsyncEnumerator())
            {
                for (var i = -1; i < index; i++)
                {
                    if (!await iterator.MoveNextAsync(cancellationToken))
                    {
                        return ReturnOrThrow<TSource>(index, throwOnEmpty);
                    }
                }

                return iterator.Current;
            }
        }

        private static TSource ReturnOrThrow<TSource>(
            int index,
            bool throwOnEmpty)
        {
            if (throwOnEmpty)
            {
                throw Error.ArgumentOutOfRange(nameof(index));
            }

            return default(TSource);
        }
    }
}
