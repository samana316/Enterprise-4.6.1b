using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        public static Task<bool> ContainsAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            TSource value)
        {
            return source.ContainsAsync(value, CancellationToken.None);
        }

        public static Task<bool> ContainsAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            TSource value,
            CancellationToken cancellationToken)
        {
            return source.ContainsAsync(value, null, cancellationToken);
        }

        public static Task<bool> ContainsAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            TSource value,
            IEqualityComparer<TSource> comparer)
        {
            return source.ContainsAsync(value, comparer, CancellationToken.None);
        }

        public static async Task<bool> ContainsAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            TSource value,
            IEqualityComparer<TSource> comparer,
            CancellationToken cancellationToken)
        {
            Check.NotNull(source, nameof(source));

            comparer = comparer ?? EqualityComparer<TSource>.Default;

            using (var enumerator = source.GetAsyncEnumerator())
            {
                while (await enumerator.MoveNextAsync(cancellationToken))
                {
                    if (comparer.Equals(value, enumerator.Current))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
