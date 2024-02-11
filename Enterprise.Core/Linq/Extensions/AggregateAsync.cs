using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Resources;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        public static Task<TSource> AggregateAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, TSource, TSource> func)
        {
            return source.AggregateAsync(func, CancellationToken.None);
        }

        public static async Task<TSource> AggregateAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, TSource, TSource> func,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(source, nameof(source));
            Check.NotNull(func, nameof(func));
            
            using (var enumerator = source.GetAsyncEnumerator())
            {
                if (!await enumerator.MoveNextAsync(cancellationToken))
                {
                    throw Error.EmptySequence();
                }

                var current = enumerator.Current;
                while (await enumerator.MoveNextAsync(cancellationToken))
                {
                    current = func(current, enumerator.Current);
                }
                return current;
            }
        }

        public static Task<TAccumulate> AggregateAsync<TSource, TAccumulate>(
            this IAsyncEnumerable<TSource> source,
            TAccumulate seed,
            Func<TAccumulate, TSource, TAccumulate> func)
        {
            return source.AggregateAsync(
                seed, func, IdentityFunction<TAccumulate>.Instance, CancellationToken.None);
        }

        public static Task<TAccumulate> AggregateAsync<TSource, TAccumulate>(
            this IAsyncEnumerable<TSource> source,
            TAccumulate seed,
            Func<TAccumulate, TSource, TAccumulate> func,
            CancellationToken cancellationToken)
        {
            return source.AggregateAsync(
                seed, func, IdentityFunction<TAccumulate>.Instance, cancellationToken);
        }

        public static Task<TResult> AggregateAsync<TSource, TAccumulate, TResult>(
            this IAsyncEnumerable<TSource> source,
            TAccumulate seed,
            Func<TAccumulate, TSource, TAccumulate> func,
            Func<TAccumulate, TResult> resultSelector)
        {
            return source.AggregateAsync(seed, func, resultSelector, CancellationToken.None);
        }

        public static async Task<TResult> AggregateAsync<TSource, TAccumulate, TResult>(
            this IAsyncEnumerable<TSource> source,
            TAccumulate seed,
            Func<TAccumulate, TSource, TAccumulate> func,
            Func<TAccumulate, TResult> resultSelector,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(source, nameof(source));
            Check.NotNull(func, nameof(func));
            Check.NotNull(resultSelector, nameof(resultSelector));

            var accumulate = seed;

            await source.ForEachAsync(current =>
            {
                accumulate = func(accumulate, current);
            },
            cancellationToken);

            return resultSelector(accumulate);
        }
    }
}