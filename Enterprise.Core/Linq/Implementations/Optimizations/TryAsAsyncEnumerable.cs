using System.Collections;
using System.Collections.Generic;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        internal static bool TryAsAsyncEnumerable(
            this IEnumerable source,
            out IAsyncEnumerable result)
        {
            result = null;

            var asyncEnumerable = source as IAsyncEnumerable;
            if (asyncEnumerable != null)
            {
                result = asyncEnumerable;
                return true;
            }

            var list = source as IList;
            if (list != null)
            {
                result = new AsyncList(list);
                return true;
            }

            var collection = source as ICollection;
            if (collection != null)
            {
                result = new AsyncCollection(collection);
                return true;
            }

            return false;
        }

        internal static bool TryAsAsyncEnumerable<TSource>(
            this IEnumerable source,
            out IAsyncEnumerable<TSource> result)
        {
            result = null;

            var asyncEnumerable = source as IAsyncEnumerable<TSource>;
            if (asyncEnumerable != null)
            {
                result = asyncEnumerable;
                return true;
            }

            var list = source as IList<TSource>;
            if (list != null)
            {
                result = new AsyncList<TSource>(list);
                return true;
            }

            var collection = source as ICollection<TSource>;
            if (collection != null)
            {
                result = new AsyncCollection<TSource>(collection);
                return true;
            }

            var readOnlyList = source as IReadOnlyList<TSource>;
            if (readOnlyList != null)
            {
                result = new AsyncReadOnlyList<TSource>(readOnlyList);
                return true;
            }

            var readOnlyCollection = source as IReadOnlyCollection<TSource>;
            if (readOnlyCollection != null)
            {
                result = new AsyncReadOnlyCollection<TSource>(readOnlyCollection);
                return true;
            }

            return false;
        }
    }
}
