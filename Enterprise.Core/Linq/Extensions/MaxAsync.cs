using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        public static Task<TSource> MaxAsync<TSource>(
            this IAsyncEnumerable<TSource> source)
        {
            return source.MaxAsync(CancellationToken.None);
        }

        public static Task<TSource> MaxAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            CancellationToken cancellationToken)
        {
            Check.NotNull(source, nameof(source));

            if (default(TSource) == null)
            {
                return source.NullableGenericMax(cancellationToken);
            }

            return source.GenericMaxAsync(cancellationToken);
        }

        public static Task<TResult> MaxAsync<TSource, TResult>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, TResult> selector)
        {
            return source.MaxAsync(selector, CancellationToken.None);
        }

        public static Task<TResult> MaxAsync<TSource, TResult>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, TResult> selector,
            CancellationToken cancellationToken)
        {
            return source.Select(selector).MaxAsync(cancellationToken);
        }

        public static Task<int> MaxAsync(
            this IAsyncEnumerable<int> source)
        {
            return source.MaxAsync(CancellationToken.None);
        }

        public static Task<int> MaxAsync(
            this IAsyncEnumerable<int> source,
            CancellationToken cancellationToken)
        {
            return source.PrimitiveMaxAsync(cancellationToken);
        }

        public static Task<int?> MaxAsync(
            this IAsyncEnumerable<int?> source)
        {
            return source.MaxAsync(CancellationToken.None);
        }

        public static Task<int?> MaxAsync(
            this IAsyncEnumerable<int?> source,
            CancellationToken cancellationToken)
        {
            return source.NullablePrimitiveMaxAsync(cancellationToken);
        }

        public static Task<int> MaxAsync<TSource>(
           this IAsyncEnumerable<TSource> source,
           Func<TSource, int> selector)
        {
            return source.MaxAsync(selector, CancellationToken.None);
        }

        public static Task<int> MaxAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, int> selector,
            CancellationToken cancellationToken)
        {
            return source.Select(selector).MaxAsync(cancellationToken);
        }

        public static Task<int?> MaxAsync<TSource>(
           this IAsyncEnumerable<TSource> source,
           Func<TSource, int?> selector)
        {
            return source.MaxAsync(selector, CancellationToken.None);
        }

        public static Task<int?> MaxAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, int?> selector,
            CancellationToken cancellationToken)
        {
            return source.Select(selector).MaxAsync(cancellationToken);
        }

        public static Task<long> MaxAsync(
           this IAsyncEnumerable<long> source)
        {
            return source.MaxAsync(CancellationToken.None);
        }

        public static Task<long> MaxAsync(
            this IAsyncEnumerable<long> source,
            CancellationToken cancellationToken)
        {
            return source.PrimitiveMaxAsync(cancellationToken);
        }

        public static Task<long?> MaxAsync(
            this IAsyncEnumerable<long?> source)
        {
            return source.MaxAsync(CancellationToken.None);
        }

        public static Task<long?> MaxAsync(
            this IAsyncEnumerable<long?> source,
            CancellationToken cancellationToken)
        {
            return source.NullablePrimitiveMaxAsync(cancellationToken);
        }

        public static Task<long> MaxAsync<TSource>(
           this IAsyncEnumerable<TSource> source,
           Func<TSource, long> selector)
        {
            return source.MaxAsync(selector, CancellationToken.None);
        }

        public static Task<long> MaxAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, long> selector,
            CancellationToken cancellationToken)
        {
            return source.Select(selector).MaxAsync(cancellationToken);
        }

        public static Task<long?> MaxAsync<TSource>(
           this IAsyncEnumerable<TSource> source,
           Func<TSource, long?> selector)
        {
            return source.MaxAsync(selector, CancellationToken.None);
        }

        public static Task<long?> MaxAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, long?> selector,
            CancellationToken cancellationToken)
        {
            return source.Select(selector).MaxAsync(cancellationToken);
        }

        public static Task<float> MaxAsync(
           this IAsyncEnumerable<float> source)
        {
            return source.MaxAsync(CancellationToken.None);
        }

        public static Task<float> MaxAsync(
            this IAsyncEnumerable<float> source,
            CancellationToken cancellationToken)
        {
            return source.PrimitiveMaxAsync(cancellationToken);
        }

        public static Task<float?> MaxAsync(
            this IAsyncEnumerable<float?> source)
        {
            return source.MaxAsync(CancellationToken.None);
        }

        public static Task<float?> MaxAsync(
            this IAsyncEnumerable<float?> source,
            CancellationToken cancellationToken)
        {
            return source.NullablePrimitiveMaxAsync(cancellationToken);
        }

        public static Task<float> MaxAsync<TSource>(
           this IAsyncEnumerable<TSource> source,
           Func<TSource, float> selector)
        {
            return source.MaxAsync(selector, CancellationToken.None);
        }

        public static Task<float> MaxAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, float> selector,
            CancellationToken cancellationToken)
        {
            return source.Select(selector).MaxAsync(cancellationToken);
        }

        public static Task<float?> MaxAsync<TSource>(
           this IAsyncEnumerable<TSource> source,
           Func<TSource, float?> selector)
        {
            return source.MaxAsync(selector, CancellationToken.None);
        }

        public static Task<float?> MaxAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, float?> selector,
            CancellationToken cancellationToken)
        {
            return source.Select(selector).MaxAsync(cancellationToken);
        }

        public static Task<double> MaxAsync(
           this IAsyncEnumerable<double> source)
        {
            return source.MaxAsync(CancellationToken.None);
        }

        public static Task<double> MaxAsync(
            this IAsyncEnumerable<double> source,
            CancellationToken cancellationToken)
        {
            return source.PrimitiveMaxAsync(cancellationToken);
        }

        public static Task<double?> MaxAsync(
            this IAsyncEnumerable<double?> source)
        {
            return source.MaxAsync(CancellationToken.None);
        }

        public static Task<double?> MaxAsync(
            this IAsyncEnumerable<double?> source,
            CancellationToken cancellationToken)
        {
            return source.NullablePrimitiveMaxAsync(cancellationToken);
        }

        public static Task<double> MaxAsync<TSource>(
           this IAsyncEnumerable<TSource> source,
           Func<TSource, double> selector)
        {
            return source.MaxAsync(selector, CancellationToken.None);
        }

        public static Task<double> MaxAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, double> selector,
            CancellationToken cancellationToken)
        {
            return source.Select(selector).MaxAsync(cancellationToken);
        }

        public static Task<double?> MaxAsync<TSource>(
           this IAsyncEnumerable<TSource> source,
           Func<TSource, double?> selector)
        {
            return source.MaxAsync(selector, CancellationToken.None);
        }

        public static Task<double?> MaxAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, double?> selector,
            CancellationToken cancellationToken)
        {
            return source.Select(selector).MaxAsync(cancellationToken);
        }

        public static Task<decimal> MaxAsync(
           this IAsyncEnumerable<decimal> source)
        {
            return source.MaxAsync(CancellationToken.None);
        }

        public static Task<decimal> MaxAsync(
            this IAsyncEnumerable<decimal> source,
            CancellationToken cancellationToken)
        {
            return source.PrimitiveMaxAsync(cancellationToken);
        }

        public static Task<decimal?> MaxAsync(
            this IAsyncEnumerable<decimal?> source)
        {
            return source.MaxAsync(CancellationToken.None);
        }

        public static Task<decimal?> MaxAsync(
            this IAsyncEnumerable<decimal?> source,
            CancellationToken cancellationToken)
        {
            return source.NullablePrimitiveMaxAsync(cancellationToken);
        }

        public static Task<decimal> MaxAsync<TSource>(
           this IAsyncEnumerable<TSource> source,
           Func<TSource, decimal> selector)
        {
            return source.MaxAsync(selector, CancellationToken.None);
        }

        public static Task<decimal> MaxAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, decimal> selector,
            CancellationToken cancellationToken)
        {
            return source.Select(selector).MaxAsync(cancellationToken);
        }

        public static Task<decimal?> MaxAsync<TSource>(
           this IAsyncEnumerable<TSource> source,
           Func<TSource, decimal?> selector)
        {
            return source.MaxAsync(selector, CancellationToken.None);
        }

        public static Task<decimal?> MaxAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, decimal?> selector,
            CancellationToken cancellationToken)
        {
            return source.Select(selector).MaxAsync(cancellationToken);
        }
    }
}
