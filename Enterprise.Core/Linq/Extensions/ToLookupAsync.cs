using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        public static Task<ILookup<TKey, TSource>> ToLookupAsync<TSource, TKey>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            return source.ToLookupAsync(
                keySelector, IdentityFunction<TSource>.Instance);
        }

        public static Task<ILookup<TKey, TSource>> ToLookupAsync<TSource, TKey>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            CancellationToken cancellationToken)
        {
            return source.ToLookupAsync(
                keySelector, IdentityFunction<TSource>.Instance, cancellationToken);
        }

        public static Task<ILookup<TKey, TSource>> ToLookupAsync<TSource, TKey>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            IEqualityComparer<TKey> comparer)
        {
            return source.ToLookupAsync(
                keySelector, IdentityFunction<TSource>.Instance, comparer);
        }

        public static Task<ILookup<TKey, TSource>> ToLookupAsync<TSource, TKey>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            IEqualityComparer<TKey> comparer,
            CancellationToken cancellationToken)
        {
            return source.ToLookupAsync(
                keySelector, IdentityFunction<TSource>.Instance, comparer, cancellationToken);
        }

        public static Task<ILookup<TKey, TElement>> ToLookupAsync<TSource, TKey, TElement>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector)
        {
            return source.ToLookupAsync(
                keySelector, elementSelector, null);
        }

        public static Task<ILookup<TKey, TElement>> ToLookupAsync<TSource, TKey, TElement>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            CancellationToken cancellationToken)
        {
            return source.ToLookupAsync(
                keySelector, elementSelector, null, cancellationToken);
        }

        public static Task<ILookup<TKey, TElement>> ToLookupAsync<TSource, TKey, TElement>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            IEqualityComparer<TKey> comparer)
        {
            return source.ToLookupAsync(
                keySelector, elementSelector, comparer, CancellationToken.None);
        }

        public static async Task<ILookup<TKey, TElement>> ToLookupAsync<TSource, TKey, TElement>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            IEqualityComparer<TKey> comparer,
            CancellationToken cancellationToken)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(keySelector, nameof(keySelector));
            Check.NotNull(elementSelector, nameof(elementSelector));

            var lookup = new Lookup<TKey, TElement>(comparer);

            await source.ForEachAsync((item) => 
            {
                var key = keySelector(item);
                var element = elementSelector(item);
                lookup.Add(key, element);
            }, cancellationToken);

            return lookup;
        }
    }
}
