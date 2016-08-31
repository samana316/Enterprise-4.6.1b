using System.Collections;
using System.Collections.Generic;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        internal static bool TryFastCount<TSource>(
            this IEnumerable<TSource> source,
            out int count)
        {
            var collection = source as ICollection<TSource>;
            if (collection != null)
            {
                count = collection.Count;
                return true;
            }

            var readOnlyCollection = source as IReadOnlyCollection<TSource>;
            if (readOnlyCollection != null)
            {
                count = readOnlyCollection.Count;
                return true;
            }

            var nonGenericCollection = source as ICollection;
            if (nonGenericCollection != null)
            {
                count = nonGenericCollection.Count;
                return true;
            }

            count = -1;
            return false;
        }
    }
}
