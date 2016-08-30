using System;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        public static IAsyncEnumerable<TSource> Reverse<TSource>(
            this IAsyncEnumerable<TSource> source)
        {
            Check.NotNull(source, nameof(source));

            return new Reverse<TSource>(source);
        }
    }
}
