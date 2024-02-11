using System.Collections.Generic;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        public static IAsyncEnumerable<TSource> Intersect<TSource>(
            this IAsyncEnumerable<TSource> first,
            IEnumerable<TSource> second)
        {
            return first.Intersect(second, null);
        }

        public static IAsyncEnumerable<TSource> Intersect<TSource>(
            this IAsyncEnumerable<TSource> first,
            IEnumerable<TSource> second,
            IEqualityComparer<TSource> comparer)
        {
            Check.NotNull(first, nameof(first));
            Check.NotNull(second, nameof(second));

            return new Intersect<TSource>(first, second, comparer);
        }
    }
}
