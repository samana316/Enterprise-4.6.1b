using System.Collections;
using System.Collections.Generic;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        public static IAsyncEnumerable AsAsyncEnumerable(
            this IAsyncEnumerable source)
        {
            return source;
        }

        public static IAsyncEnumerable AsAsyncEnumerable(
            this IEnumerable source)
        {
            if (source == null)
            {
                return null;
            }

            IAsyncEnumerable asyncEnumerable;
            if (source.TryAsAsyncEnumerable(out asyncEnumerable))
            {
                return asyncEnumerable;
            }

            return new AsAsyncEnumerable(source);
        }

        public static IAsyncEnumerable<TSource> AsAsyncEnumerable<TSource>(
            this IAsyncEnumerable<TSource> source)
        {
            return source;
        }

        public static IAsyncEnumerable<TSource> AsAsyncEnumerable<TSource>(
            this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                return null;
            }

            IAsyncEnumerable<TSource> asyncEnumerable;
            if (source.TryAsAsyncEnumerable(out asyncEnumerable))
            {
                return asyncEnumerable;
            }

            return new AsAsyncEnumerable<TSource>(source);
        }
    }
}
