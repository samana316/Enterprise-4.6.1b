using System.Collections;
using System.Collections.Generic;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        internal static bool TryFastIndexer<TSource>(
            this IEnumerable<TSource> source,
            int index,
            out TSource result)
        {
            result = default(TSource);

            var list = source as IList<TSource>;
            if (list != null)
            {
                result = list[index];
                return true;
            }

            var readOnlyList = source as IReadOnlyList<TSource>;
            if (readOnlyList != null)
            {
                result = readOnlyList[index];
                return true;
            }

            var nonGenericList = source as IList;
            if (nonGenericList != null)
            {
                result = (TSource)nonGenericList[index];
                return true;
            }

            return false;
        }
    }
}
