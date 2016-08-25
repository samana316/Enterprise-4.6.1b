using System.Collections.Generic;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
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

            var asyncEnumerable = source as IAsyncEnumerable<TSource>;
            if (asyncEnumerable != null)
            {
                return asyncEnumerable;
            }

            return new AsAsyncEnumerable<TSource>(source);
        }
    }
}
