using System;
using System.Collections.Generic;
using System.Linq;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        public static IAsyncEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            return source.GroupBy(keySelector, IdentityFunction<TSource>.Instance, null);
        }

        public static IAsyncEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            IEqualityComparer<TKey> comparer)
        {
            return source.GroupBy(keySelector, IdentityFunction<TSource>.Instance, comparer);
        }

        public static IAsyncEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector)
        {
            return source.GroupBy(keySelector, elementSelector, null);
        }

        public static IAsyncEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            IEqualityComparer<TKey> comparer)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(keySelector, nameof(keySelector));
            Check.NotNull(elementSelector, nameof(elementSelector));

            return new GroupBy<TSource, TKey, TElement>(source, keySelector, elementSelector, comparer);
        }

        public static IAsyncEnumerable<TResult> GroupBy<TSource, TKey, TResult>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TKey, IEnumerable<TSource>, TResult> resultSelector)
        {
            return source.GroupBy(keySelector, resultSelector, null);
        }

        public static IAsyncEnumerable<TResult> GroupBy<TSource, TKey, TResult>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TKey, IEnumerable<TSource>, TResult> resultSelector,
            IEqualityComparer<TKey> comparer)
        {
            return source.GroupBy(keySelector, IdentityFunction<TSource>.Instance, resultSelector, comparer);
        }

        public static IAsyncEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            Func<TKey, IEnumerable<TElement>, TResult> resultSelector)
        {
            return source.GroupBy(keySelector, elementSelector, resultSelector, null);
        }

        public static IAsyncEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            Func<TKey, IEnumerable<TElement>, TResult> resultSelector,
            IEqualityComparer<TKey> comparer)
        {
            Check.NotNull(resultSelector, nameof(resultSelector));

            var groups = source.GroupBy(keySelector, elementSelector, comparer);

            return
                from grouping in groups
                select resultSelector(grouping.Key, grouping);
        }
    }
}
