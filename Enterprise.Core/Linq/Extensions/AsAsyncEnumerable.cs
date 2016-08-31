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

            var list = source as IList<TSource>;
            if (list != null)
            {
                return new AsyncList<TSource>(list);
            }

            var collection = source as ICollection<TSource>;
            if (collection != null)
            {
                return new AsyncCollection<TSource>(collection);
            }

            var readOnlyList = source as IReadOnlyList<TSource>;
            if (readOnlyList != null)
            {
                return new AsyncReadOnlyList<TSource>(readOnlyList);
            }

            var readOnlyCollection = source as IReadOnlyCollection<TSource>;
            if (readOnlyCollection != null)
            {
                return new AsyncReadOnlyCollection<TSource>(readOnlyCollection);
            }

            return new AsAsyncEnumerable<TSource>(source);
        }
    }
}
