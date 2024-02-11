using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        public static Task<TSource> MinAsync<TSource>(
            this IAsyncEnumerable<TSource> source)
        {
            return source.MinAsync(CancellationToken.None);
        }

        public static Task<TSource> MinAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            CancellationToken cancellationToken)
        {
            Check.NotNull(source, nameof(source));

            if (default(TSource) == null)
            {
                return source.NullableGenericMinAsync(cancellationToken);
            }

            return source.GenericMinAsync(cancellationToken);
        }

        public static Task<TResult> MinAsync<TSource, TResult>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, TResult> selector)
        {
            return source.MinAsync(selector, CancellationToken.None);
        }

        public static Task<TResult> MinAsync<TSource, TResult>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, TResult> selector,
            CancellationToken cancellationToken)
        {
            return source.Select(selector).MinAsync(cancellationToken);
        }

        public static Task<int> MinAsync(
            this IAsyncEnumerable<int> source)
        {
            return source.MinAsync(CancellationToken.None);
        }

        public static Task<int> MinAsync(
            this IAsyncEnumerable<int> source,
            CancellationToken cancellationToken)
        {
            return source.PrimitiveMinAsync(cancellationToken);
        }

        public static Task<int?> MinAsync(
            this IAsyncEnumerable<int?> source)
        {
            return source.MinAsync(CancellationToken.None);
        }

        public static Task<int?> MinAsync(
            this IAsyncEnumerable<int?> source,
            CancellationToken cancellationToken)
        {
            return source.NullablePrimitiveMinAsync(cancellationToken);
        }

        public static Task<int> MinAsync<TSource>(
           this IAsyncEnumerable<TSource> source,
           Func<TSource, int> selector)
        {
            return source.MinAsync(selector, CancellationToken.None);
        }

        public static Task<int> MinAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, int> selector,
            CancellationToken cancellationToken)
        {
            return source.Select(selector).MinAsync(cancellationToken);
        }

        public static Task<int?> MinAsync<TSource>(
           this IAsyncEnumerable<TSource> source,
           Func<TSource, int?> selector)
        {
            return source.MinAsync(selector, CancellationToken.None);
        }

        public static Task<int?> MinAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, int?> selector,
            CancellationToken cancellationToken)
        {
            return source.Select(selector).MinAsync(cancellationToken);
        }

        public static Task<long> MinAsync(
           this IAsyncEnumerable<long> source)
        {
            return source.MinAsync(CancellationToken.None);
        }

        public static Task<long> MinAsync(
            this IAsyncEnumerable<long> source,
            CancellationToken cancellationToken)
        {
            return source.PrimitiveMinAsync(cancellationToken);
        }

        public static Task<long?> MinAsync(
            this IAsyncEnumerable<long?> source)
        {
            return source.MinAsync(CancellationToken.None);
        }

        public static Task<long?> MinAsync(
            this IAsyncEnumerable<long?> source,
            CancellationToken cancellationToken)
        {
            return source.NullablePrimitiveMinAsync(cancellationToken);
        }

        public static Task<long> MinAsync<TSource>(
           this IAsyncEnumerable<TSource> source,
           Func<TSource, long> selector)
        {
            return source.MinAsync(selector, CancellationToken.None);
        }

        public static Task<long> MinAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, long> selector,
            CancellationToken cancellationToken)
        {
            return source.Select(selector).MinAsync(cancellationToken);
        }

        public static Task<long?> MinAsync<TSource>(
           this IAsyncEnumerable<TSource> source,
           Func<TSource, long?> selector)
        {
            return source.MinAsync(selector, CancellationToken.None);
        }

        public static Task<long?> MinAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, long?> selector,
            CancellationToken cancellationToken)
        {
            return source.Select(selector).MinAsync(cancellationToken);
        }

        public static Task<float> MinAsync(
           this IAsyncEnumerable<float> source)
        {
            return source.MinAsync(CancellationToken.None);
        }

        public static Task<float> MinAsync(
            this IAsyncEnumerable<float> source,
            CancellationToken cancellationToken)
        {
            return source.PrimitiveMinAsync(cancellationToken);
        }

        public static Task<float?> MinAsync(
            this IAsyncEnumerable<float?> source)
        {
            return source.MinAsync(CancellationToken.None);
        }

        public static Task<float?> MinAsync(
            this IAsyncEnumerable<float?> source,
            CancellationToken cancellationToken)
        {
            return source.NullablePrimitiveMinAsync(cancellationToken);
        }

        public static Task<float> MinAsync<TSource>(
           this IAsyncEnumerable<TSource> source,
           Func<TSource, float> selector)
        {
            return source.MinAsync(selector, CancellationToken.None);
        }

        public static Task<float> MinAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, float> selector,
            CancellationToken cancellationToken)
        {
            return source.Select(selector).MinAsync(cancellationToken);
        }

        public static Task<float?> MinAsync<TSource>(
           this IAsyncEnumerable<TSource> source,
           Func<TSource, float?> selector)
        {
            return source.MinAsync(selector, CancellationToken.None);
        }

        public static Task<float?> MinAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, float?> selector,
            CancellationToken cancellationToken)
        {
            return source.Select(selector).MinAsync(cancellationToken);
        }

        public static Task<double> MinAsync(
           this IAsyncEnumerable<double> source)
        {
            return source.MinAsync(CancellationToken.None);
        }

        public static Task<double> MinAsync(
            this IAsyncEnumerable<double> source,
            CancellationToken cancellationToken)
        {
            return source.PrimitiveMinAsync(cancellationToken);
        }

        public static Task<double?> MinAsync(
            this IAsyncEnumerable<double?> source)
        {
            return source.MinAsync(CancellationToken.None);
        }

        public static Task<double?> MinAsync(
            this IAsyncEnumerable<double?> source,
            CancellationToken cancellationToken)
        {
            return source.NullablePrimitiveMinAsync(cancellationToken);
        }

        public static Task<double> MinAsync<TSource>(
           this IAsyncEnumerable<TSource> source,
           Func<TSource, double> selector)
        {
            return source.MinAsync(selector, CancellationToken.None);
        }

        public static Task<double> MinAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, double> selector,
            CancellationToken cancellationToken)
        {
            return source.Select(selector).MinAsync(cancellationToken);
        }

        public static Task<double?> MinAsync<TSource>(
           this IAsyncEnumerable<TSource> source,
           Func<TSource, double?> selector)
        {
            return source.MinAsync(selector, CancellationToken.None);
        }

        public static Task<double?> MinAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, double?> selector,
            CancellationToken cancellationToken)
        {
            return source.Select(selector).MinAsync(cancellationToken);
        }

        public static Task<decimal> MinAsync(
           this IAsyncEnumerable<decimal> source)
        {
            return source.MinAsync(CancellationToken.None);
        }

        public static Task<decimal> MinAsync(
            this IAsyncEnumerable<decimal> source,
            CancellationToken cancellationToken)
        {
            return source.PrimitiveMinAsync(cancellationToken);
        }

        public static Task<decimal?> MinAsync(
            this IAsyncEnumerable<decimal?> source)
        {
            return source.MinAsync(CancellationToken.None);
        }

        public static Task<decimal?> MinAsync(
            this IAsyncEnumerable<decimal?> source,
            CancellationToken cancellationToken)
        {
            return source.NullablePrimitiveMinAsync(cancellationToken);
        }

        public static Task<decimal> MinAsync<TSource>(
           this IAsyncEnumerable<TSource> source,
           Func<TSource, decimal> selector)
        {
            return source.MinAsync(selector, CancellationToken.None);
        }

        public static Task<decimal> MinAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, decimal> selector,
            CancellationToken cancellationToken)
        {
            return source.Select(selector).MinAsync(cancellationToken);
        }

        public static Task<decimal?> MinAsync<TSource>(
           this IAsyncEnumerable<TSource> source,
           Func<TSource, decimal?> selector)
        {
            return source.MinAsync(selector, CancellationToken.None);
        }

        public static Task<decimal?> MinAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, decimal?> selector,
            CancellationToken cancellationToken)
        {
            return source.Select(selector).MinAsync(cancellationToken);
        }
    }
}
