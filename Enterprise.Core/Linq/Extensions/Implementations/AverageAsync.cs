using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Resources;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        internal static Task<double> AverageAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, TSource, TSource> adder,
            CancellationToken cancellationToken)
            where TSource : struct, IConvertible
        {
            return source.AverageAsync(IdentityFunction<TSource>.Instance, adder, cancellationToken);
        }

        internal static async Task<double> AverageAsync<TSource, TElement>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, TElement> selector,
            Func<TElement, TElement, TElement> adder,
            CancellationToken cancellationToken)
            where TElement : struct, IConvertible
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));

            long count = 0;
            var total = default(TElement);

            checked
            {
                await source.ForEachAsync(current =>
                {
                    var item = selector(current);

                    count++;
                    total = adder(total, item);
                }, cancellationToken);

                if (count == 0)
                {
                    throw Error.EmptySequence();
                }

                return Convert.ToDouble(total) / count;
            }
        }

        internal static Task<double?> AverageAsync<TSource>(
            this IAsyncEnumerable<TSource?> source,
            Func<TSource, TSource, TSource> adder,
            CancellationToken cancellationToken)
            where TSource : struct, IConvertible
        {
            return source.AverageAsync(IdentityFunction<TSource>.Instance, adder, cancellationToken);
        }

        internal static async Task<double?> AverageAsync<TSource, TElement>(
            this IAsyncEnumerable<TSource?> source,
            Func<TSource, TElement> selector,
            Func<TElement, TElement, TElement> adder,
            CancellationToken cancellationToken)
            where TSource : struct, IConvertible
            where TElement : struct, IConvertible
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));

            long count = 0;
            var total = default(TElement);

            checked
            {
                await source.ForEachAsync(current =>
                {
                    if (current.HasValue)
                    {
                        var item = selector(current.Value);

                        count++;
                        total = adder(total, item);
                    }
                }, cancellationToken);

                return count == 0 ? (double?)null : Convert.ToDouble(total) / count;
            }
        }
    }
}
