using System.Collections.Generic;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        public static IAsyncEnumerable<TSource> Distinct<TSource>(
            this IAsyncEnumerable<TSource> source)
        {
            return source.Distinct(null);
        }

        public static IAsyncEnumerable<TSource> Distinct<TSource>(
            this IAsyncEnumerable<TSource> source,
            IEqualityComparer<TSource> comparer)
        {
            Check.NotNull(source, nameof(source));

            return new Distinct<TSource>(source, comparer);
        }
    }
}
