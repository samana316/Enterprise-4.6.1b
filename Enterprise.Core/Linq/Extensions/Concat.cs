using System.Collections.Generic;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        public static IAsyncEnumerable<TSource> Concat<TSource>(
            this IAsyncEnumerable<TSource> first,
            IEnumerable<TSource> second)
        {
            Check.NotNull(first, nameof(first));
            Check.NotNull(second, nameof(second));

            return new Concat<TSource>(first, second);
        }
    }
}
