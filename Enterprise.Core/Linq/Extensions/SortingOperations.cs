using System;
using System.Collections.Generic;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        public static IAsyncOrderedEnumerable<TSource> OrderBy<TSource, TKey>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            return new AsyncOrderedEnumerable<TSource, TKey>(source, keySelector, null);
        }

        public static IAsyncOrderedEnumerable<TSource> OrderBy<TSource, TKey>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            IComparer<TKey> comparer)
        {
            return new AsyncOrderedEnumerable<TSource, TKey>(source, keySelector, comparer);
        }

        public static IAsyncOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            return new AsyncOrderedEnumerable<TSource, TKey>(source, keySelector, null, true);
        }

        public static IAsyncOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            IComparer<TKey> comparer)
        {
            return new AsyncOrderedEnumerable<TSource, TKey>(source, keySelector, comparer, true);
        }

        public static IAsyncOrderedEnumerable<TSource> ThenBy<TSource, TKey>(
            this IAsyncOrderedEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            Check.NotNull(source, nameof(source));

            return source.CreateAsyncOrderedEnumerable(keySelector, null, false);
        }

        public static IAsyncOrderedEnumerable<TSource> ThenBy<TSource, TKey>(
            this IAsyncOrderedEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            IComparer<TKey> comparer)
        {
            Check.NotNull(source, nameof(source));

            return source.CreateAsyncOrderedEnumerable(keySelector, comparer, false);
        }

        public static IAsyncOrderedEnumerable<TSource> ThenByDescending<TSource, TKey>(
            this IAsyncOrderedEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            Check.NotNull(source, nameof(source));

            return source.CreateAsyncOrderedEnumerable(keySelector, null, true);
        }

        public static IAsyncOrderedEnumerable<TSource> ThenByDescending<TSource, TKey>(
            this IAsyncOrderedEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            IComparer<TKey> comparer)
        {
            Check.NotNull(source, nameof(source));

            return source.CreateAsyncOrderedEnumerable(keySelector, comparer, true);
        }
    }
}
