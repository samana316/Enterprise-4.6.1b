using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        internal static async Task<TResult> SumAsync<TSource, TResult>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, TResult> selector,
            Func<TResult, TResult, TResult> adder,
            CancellationToken cancellationToken)
            where TResult : struct
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));

            checked
            {
                var sum = default(TResult);

                await source.ForEachAsync(item =>
                {
                    var value = selector(item);

                    sum = adder(sum, value);
                }, cancellationToken);

                return sum;
            }
        }

        internal static async Task<TResult?> SumAsync<TSource, TResult>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, TResult?> selector,
            Func<TResult, TResult, TResult> adder,
            CancellationToken cancellationToken)
            where TResult : struct
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));

            checked
            {
                var sum = default(TResult);

                await source.ForEachAsync(item => 
                {
                    var value = selector(item);

                    sum = adder(sum, value.GetValueOrDefault());
                }, cancellationToken);

                return sum;
            }
        }

        private static class Adder
        {
            public static int Int32(
                int x,
                int y)
            {
                checked { return x + y; }
            }

            public static long Int64(
                long x,
                long y)
            {
                checked { return x + y; }
            }

            public static float Single(
                float x,
                float y)
            {
                checked { return x + y; }
            }

            public static double Double(
                double x,
                double y)
            {
                checked { return x + y; }
            }

            public static decimal Decimal(
                decimal x,
                decimal y)
            {
                checked { return x + y; }
            }
        }
    }
}
