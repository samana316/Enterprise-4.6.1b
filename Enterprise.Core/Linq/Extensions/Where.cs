using System;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        public static IAsyncEnumerable<TSource> Where<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, bool> predicate)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(predicate, nameof(predicate));

            return new Where<TSource>(source, predicate);
        }

        public static IAsyncEnumerable<TSource> Where<TSource>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, int, bool> predicate)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(predicate, nameof(predicate));

            return new Where<TSource>(source, predicate);
        }
    }
}
