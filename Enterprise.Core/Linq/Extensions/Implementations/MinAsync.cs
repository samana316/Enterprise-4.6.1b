using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Resources;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        private static async Task<T> PrimitiveMinAsync<T>(
            this IAsyncEnumerable<T> source,
            CancellationToken cancellationToken)
            where T : struct, IComparable<T>, IComparable
        {
            Check.NotNull(source, nameof(source));

            using (var iterator = source.GetAsyncEnumerator())
            {
                if (!await iterator.MoveNextAsync(cancellationToken))
                {
                    throw Error.EmptySequence();
                }

                var max = iterator.Current;
                while (await iterator.MoveNextAsync(cancellationToken))
                {
                    var item = iterator.Current;
                    if (max.CompareTo(item) > 0)
                    {
                        max = item;
                    }
                }

                return max;
            }
        }

        private static async Task<T?> NullablePrimitiveMinAsync<T>(
            this IAsyncEnumerable<T?> source,
            CancellationToken cancellationToken)
            where T : struct, IComparable<T>, IComparable
        {
            Check.NotNull(source, nameof(source));

            var max = default(T?);
            await source.ForEachAsync(item =>
            {
                if (item != null && (max == null || max.Value.CompareTo(item.Value) > 0))
                {
                    max = item;
                }
            }, cancellationToken);

            return max;
        }

        private static async Task<T> GenericMinAsync<T>(
            this IAsyncEnumerable<T> source,
            CancellationToken cancellationToken)
        {
            var comparer = Comparer<T>.Default;

            using (var iterator = source.GetAsyncEnumerator())
            {
                if (!await iterator.MoveNextAsync(cancellationToken))
                {
                    throw Error.EmptySequence();
                }

                var max = iterator.Current;
                while (await iterator.MoveNextAsync(cancellationToken))
                {
                    var item = iterator.Current;
                    if (comparer.Compare(max, item) > 0)
                    {
                        max = item;
                    }
                }
                return max;
            }
        }

        private static async Task<T> NullableGenericMinAsync<T>(
            this IAsyncEnumerable<T> source,
            CancellationToken cancellationToken)
        {
            var comparer = Comparer<T>.Default;

            var max = default(T);
            await source.ForEachAsync(item =>
            {
                if (item != null && (max == null || comparer.Compare(max, item) > 0))
                {
                    max = item;
                }
            }, cancellationToken);

            return max;
        }
    }
}
