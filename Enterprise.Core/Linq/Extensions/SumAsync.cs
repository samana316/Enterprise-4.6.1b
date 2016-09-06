using System;
using System.Threading;
using System.Threading.Tasks;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        public static Task<int> SumAsync(
            this IAsyncEnumerable<int> source)
        {
            return source.SumAsync(CancellationToken.None);
        }

        public static Task<int> SumAsync(
            this IAsyncEnumerable<int> source,
            CancellationToken cancellationToken)
        {
            return source.SumAsync(IdentityFunction<int>.Instance, Adder.Int32, cancellationToken);
        }

        public static Task<int?> SumAsync(
            this IAsyncEnumerable<int?> source)
        {
            return source.SumAsync(CancellationToken.None);
        }

        public static Task<int?> SumAsync(
            this IAsyncEnumerable<int?> source,
            CancellationToken cancellationToken)
        {
            return source.SumAsync(IdentityFunction<int?>.Instance, Adder.Int32, cancellationToken);
        }

        public static Task<int> SumAsync<TSource>(
           this IAsyncEnumerable<TSource> source,
           Func<TSource, int> selector)
        {
            return source.SumAsync(selector, CancellationToken.None);
        }

        public static Task<int> SumAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, int> selector,
            CancellationToken cancellationToken)
        {
            return source.SumAsync(selector, Adder.Int32, cancellationToken);
        }

        public static Task<int?> SumAsync<TSource>(
           this IAsyncEnumerable<TSource> source,
           Func<TSource, int?> selector)
        {
            return source.SumAsync(selector, CancellationToken.None);
        }

        public static Task<int?> SumAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, int?> selector,
            CancellationToken cancellationToken)
        {
            return source.SumAsync(selector, Adder.Int32, cancellationToken);
        }

        public static Task<long> SumAsync(
            this IAsyncEnumerable<long> source)
        {
            return source.SumAsync(CancellationToken.None);
        }

        public static Task<long> SumAsync(
            this IAsyncEnumerable<long> source,
            CancellationToken cancellationToken)
        {
            return source.SumAsync(IdentityFunction<long>.Instance, Adder.Int64, cancellationToken);
        }

        public static Task<long?> SumAsync(
            this IAsyncEnumerable<long?> source)
        {
            return source.SumAsync(CancellationToken.None);
        }

        public static Task<long?> SumAsync(
            this IAsyncEnumerable<long?> source,
            CancellationToken cancellationToken)
        {
            return source.SumAsync(IdentityFunction<long?>.Instance, Adder.Int64, cancellationToken);
        }

        public static Task<long> SumAsync<TSource>(
           this IAsyncEnumerable<TSource> source,
           Func<TSource, long> selector)
        {
            return source.SumAsync(selector, CancellationToken.None);
        }

        public static Task<long> SumAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, long> selector,
            CancellationToken cancellationToken)
        {
            return source.SumAsync(selector, Adder.Int64, cancellationToken);
        }

        public static Task<long?> SumAsync<TSource>(
           this IAsyncEnumerable<TSource> source,
           Func<TSource, long?> selector)
        {
            return source.SumAsync(selector, CancellationToken.None);
        }

        public static Task<long?> SumAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, long?> selector,
            CancellationToken cancellationToken)
        {
            return source.SumAsync(selector, Adder.Int64, cancellationToken);
        }

        public static Task<float> SumAsync(
            this IAsyncEnumerable<float> source)
        {
            return source.SumAsync(CancellationToken.None);
        }

        public static Task<float> SumAsync(
            this IAsyncEnumerable<float> source,
            CancellationToken cancellationToken)
        {
            return source.SumAsync(IdentityFunction<float>.Instance, Adder.Single, cancellationToken);
        }

        public static Task<float?> SumAsync(
            this IAsyncEnumerable<float?> source)
        {
            return source.SumAsync(CancellationToken.None);
        }

        public static Task<float?> SumAsync(
            this IAsyncEnumerable<float?> source,
            CancellationToken cancellationToken)
        {
            return source.SumAsync(IdentityFunction<float?>.Instance, Adder.Single, cancellationToken);
        }

        public static Task<float> SumAsync<TSource>(
           this IAsyncEnumerable<TSource> source,
           Func<TSource, float> selector)
        {
            return source.SumAsync(selector, CancellationToken.None);
        }

        public static Task<float> SumAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, float> selector,
            CancellationToken cancellationToken)
        {
            return source.SumAsync(selector, Adder.Single, cancellationToken);
        }

        public static Task<float?> SumAsync<TSource>(
           this IAsyncEnumerable<TSource> source,
           Func<TSource, float?> selector)
        {
            return source.SumAsync(selector, CancellationToken.None);
        }

        public static Task<float?> SumAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, float?> selector,
            CancellationToken cancellationToken)
        {
            return source.SumAsync(selector, Adder.Single, cancellationToken);
        }

        public static Task<double> SumAsync(
            this IAsyncEnumerable<double> source)
        {
            return source.SumAsync(CancellationToken.None);
        }

        public static Task<double> SumAsync(
            this IAsyncEnumerable<double> source,
            CancellationToken cancellationToken)
        {
            return source.SumAsync(IdentityFunction<double>.Instance, Adder.Double, cancellationToken);
        }

        public static Task<double?> SumAsync(
            this IAsyncEnumerable<double?> source)
        {
            return source.SumAsync(CancellationToken.None);
        }

        public static Task<double?> SumAsync(
            this IAsyncEnumerable<double?> source,
            CancellationToken cancellationToken)
        {
            return source.SumAsync(IdentityFunction<double?>.Instance, Adder.Double, cancellationToken);
        }

        public static Task<double> SumAsync<TSource>(
           this IAsyncEnumerable<TSource> source,
           Func<TSource, double> selector)
        {
            return source.SumAsync(selector, CancellationToken.None);
        }

        public static Task<double> SumAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, double> selector,
            CancellationToken cancellationToken)
        {
            return source.SumAsync(selector, Adder.Double, cancellationToken);
        }

        public static Task<double?> SumAsync<TSource>(
           this IAsyncEnumerable<TSource> source,
           Func<TSource, double?> selector)
        {
            return source.SumAsync(selector, CancellationToken.None);
        }

        public static Task<double?> SumAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, double?> selector,
            CancellationToken cancellationToken)
        {
            return source.SumAsync(selector, Adder.Double, cancellationToken);
        }

        public static Task<decimal> SumAsync(
            this IAsyncEnumerable<decimal> source)
        {
            return source.SumAsync(CancellationToken.None);
        }

        public static Task<decimal> SumAsync(
            this IAsyncEnumerable<decimal> source,
            CancellationToken cancellationToken)
        {
            return source.SumAsync(IdentityFunction<decimal>.Instance, Adder.Decimal, cancellationToken);
        }

        public static Task<decimal?> SumAsync(
            this IAsyncEnumerable<decimal?> source)
        {
            return source.SumAsync(CancellationToken.None);
        }

        public static Task<decimal?> SumAsync(
            this IAsyncEnumerable<decimal?> source,
            CancellationToken cancellationToken)
        {
            return source.SumAsync(IdentityFunction<decimal?>.Instance, Adder.Decimal, cancellationToken);
        }

        public static Task<decimal> SumAsync<TSource>(
           this IAsyncEnumerable<TSource> source,
           Func<TSource, decimal> selector)
        {
            return source.SumAsync(selector, CancellationToken.None);
        }

        public static Task<decimal> SumAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, decimal> selector,
            CancellationToken cancellationToken)
        {
            return source.SumAsync(selector, Adder.Decimal, cancellationToken);
        }

        public static Task<decimal?> SumAsync<TSource>(
           this IAsyncEnumerable<TSource> source,
           Func<TSource, decimal?> selector)
        {
            return source.SumAsync(selector, CancellationToken.None);
        }

        public static Task<decimal?> SumAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, decimal?> selector,
            CancellationToken cancellationToken)
        {
            return source.SumAsync(selector, Adder.Decimal, cancellationToken);
        }
    }
}
