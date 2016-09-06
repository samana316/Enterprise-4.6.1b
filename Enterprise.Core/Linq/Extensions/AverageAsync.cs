using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Resources;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        public static Task<double> AverageAsync(
            this IAsyncEnumerable<int> source)
        {
            return source.AverageAsync(CancellationToken.None);
        }

        public static Task<double> AverageAsync(
            this IAsyncEnumerable<int> source,
            CancellationToken cancellationToken)
        {
            return source.AverageAsync(Convert.ToInt64, Adder.Int64, cancellationToken);
        }

        public static Task<double> AverageAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, int> selector)
        {
            return source.AverageAsync(selector, CancellationToken.None);
        }

        public static Task<double> AverageAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, int> selector,
            CancellationToken cancellationToken)
        {
            return source.Select(selector).AverageAsync(cancellationToken);
        }

        public static Task<double?> AverageAsync(
            this IAsyncEnumerable<int?> source)
        {
            return source.AverageAsync(CancellationToken.None);
        }

        public static Task<double?> AverageAsync(
            this IAsyncEnumerable<int?> source,
            CancellationToken cancellationToken)
        {
            return source.AverageAsync<int, long>(Convert.ToInt64, Adder.Int64, cancellationToken);
        }

        public static Task<double?> AverageAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, int?> selector)
        {
            return source.AverageAsync(selector, CancellationToken.None);
        }

        public static Task<double?> AverageAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, int?> selector,
            CancellationToken cancellationToken)
        {
            return source.Select(selector).AverageAsync(cancellationToken);
        }

        public static Task<double> AverageAsync(
            this IAsyncEnumerable<long> source)
        {
            return source.AverageAsync(CancellationToken.None);
        }

        public static Task<double> AverageAsync(
            this IAsyncEnumerable<long> source,
            CancellationToken cancellationToken)
        {
            return source.AverageAsync(Adder.Int64, cancellationToken);
        }

        public static Task<double> AverageAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, long> selector)
        {
            return source.AverageAsync(selector, CancellationToken.None);
        }

        public static Task<double> AverageAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, long> selector,
            CancellationToken cancellationToken)
        {
            return source.Select(selector).AverageAsync(cancellationToken);
        }

        public static Task<double?> AverageAsync(
            this IAsyncEnumerable<long?> source)
        {
            return source.AverageAsync(CancellationToken.None);
        }

        public static Task<double?> AverageAsync(
            this IAsyncEnumerable<long?> source,
            CancellationToken cancellationToken)
        {
            return source.AverageAsync(Adder.Int64, cancellationToken);
        }

        public static Task<double?> AverageAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, long?> selector)
        {
            return source.AverageAsync(selector, CancellationToken.None);
        }

        public static Task<double?> AverageAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, long?> selector,
            CancellationToken cancellationToken)
        {
            return source.Select(selector).AverageAsync(cancellationToken);
        }

        public static Task<double> AverageAsync(
            this IAsyncEnumerable<float> source)
        {
            return source.AverageAsync(CancellationToken.None);
        }

        public static Task<double> AverageAsync(
            this IAsyncEnumerable<float> source,
            CancellationToken cancellationToken)
        {
            return source.AverageAsync(Convert.ToDouble, Adder.Double, cancellationToken);
        }

        public static Task<double> AverageAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, float> selector)
        {
            return source.AverageAsync(selector, CancellationToken.None);
        }

        public static Task<double> AverageAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, float> selector,
            CancellationToken cancellationToken)
        {
            return source.Select(selector).AverageAsync(cancellationToken);
        }

        public static Task<double?> AverageAsync(
            this IAsyncEnumerable<float?> source)
        {
            return source.AverageAsync(CancellationToken.None);
        }

        public static Task<double?> AverageAsync(
            this IAsyncEnumerable<float?> source,
            CancellationToken cancellationToken)
        {
            return source.AverageAsync<float, double>(Convert.ToDouble, Adder.Double, cancellationToken);
        }

        public static Task<double?> AverageAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, float?> selector)
        {
            return source.AverageAsync(selector, CancellationToken.None);
        }

        public static Task<double?> AverageAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, float?> selector,
            CancellationToken cancellationToken)
        {
            return source.Select(selector).AverageAsync(cancellationToken);
        }

        public static Task<double> AverageAsync(
            this IAsyncEnumerable<double> source)
        {
            return source.AverageAsync(CancellationToken.None);
        }

        public static Task<double> AverageAsync(
            this IAsyncEnumerable<double> source,
            CancellationToken cancellationToken)
        {
            return source.AverageAsync(Adder.Double, cancellationToken);
        }

        public static Task<double> AverageAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, double> selector)
        {
            return source.AverageAsync(selector, CancellationToken.None);
        }

        public static Task<double> AverageAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, double> selector,
            CancellationToken cancellationToken)
        {
            return source.Select(selector).AverageAsync(cancellationToken);
        }

        public static Task<double?> AverageAsync(
            this IAsyncEnumerable<double?> source)
        {
            return source.AverageAsync(CancellationToken.None);
        }

        public static Task<double?> AverageAsync(
            this IAsyncEnumerable<double?> source,
            CancellationToken cancellationToken)
        {
            return source.AverageAsync(Adder.Double, cancellationToken);
        }

        public static Task<double?> AverageAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, double?> selector)
        {
            return source.AverageAsync(selector, CancellationToken.None);
        }

        public static Task<double?> AverageAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, double?> selector,
            CancellationToken cancellationToken)
        {
            return source.Select(selector).AverageAsync(cancellationToken);
        }

        public static Task<decimal> AverageAsync(
            this IAsyncEnumerable<decimal> source)
        {
            return source.AverageAsync(CancellationToken.None);
        }

        public static async Task<decimal> AverageAsync(
            this IAsyncEnumerable<decimal> source,
            CancellationToken cancellationToken)
        {
            Check.NotNull(source, nameof(source));

            long count = 0;
            var total = default(decimal);

            await source.ForEachAsync(item =>
            {
                checked
                {
                    count++;
                    total += item;
                }
            }, cancellationToken);

            if (count == 0)
            {
                throw Error.EmptySequence();
            }

            return total / Convert.ToDecimal(count);
        }

        public static Task<decimal> AverageAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, decimal> selector)
        {
            return source.AverageAsync(selector, CancellationToken.None);
        }

        public static Task<decimal> AverageAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, decimal> selector,
            CancellationToken cancellationToken)
        {
            return source.Select(selector).AverageAsync(cancellationToken);
        }

        public static Task<decimal?> AverageAsync(
            this IAsyncEnumerable<decimal?> source)
        {
            return source.AverageAsync(CancellationToken.None);
        }

        public static async Task<decimal?> AverageAsync(
            this IAsyncEnumerable<decimal?> source,
            CancellationToken cancellationToken)
        {
            Check.NotNull(source, nameof(source));

            long count = 0;
            var total = default(decimal);

            await source.ForEachAsync(item =>
            {
                if (item.HasValue)
                {
                    checked
                    {
                        count++;
                        total += item.Value;
                    }
                }
            }, cancellationToken);

            if (count == 0)
            {
                return null;
            }

            return total / Convert.ToDecimal(count);
        }

        public static Task<decimal?> AverageAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, decimal?> selector)
        {
            return source.AverageAsync(selector, CancellationToken.None);
        }

        public static Task<decimal?> AverageAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, decimal?> selector,
            CancellationToken cancellationToken)
        {
            return source.Select(selector).AverageAsync(cancellationToken);
        }
    }
}
