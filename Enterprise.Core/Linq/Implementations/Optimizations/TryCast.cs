using System.Collections;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        private static bool TryCast<TResult>(
           this IAsyncEnumerable source,
           out IAsyncEnumerable<TResult> result)
        {
            result = null;

            var list = source as IList;
            if (list != null)
            {
                result = new AsyncCastList<TResult>(list);
                return true;
            }

            var collection = source as ICollection;
            if (collection != null)
            {
                result = new AsyncCastCollection<TResult>(collection);
                return true;
            }

            return false;
        }
    }
}
