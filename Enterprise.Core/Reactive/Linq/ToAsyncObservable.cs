using System.Collections.Generic;
using Enterprise.Core.Reactive.Linq.Implementations;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Reactive.Linq
{
    partial class AsyncObservable
    {
        public static IAsyncObservable<TSource> ToAsyncObservable<TSource>(
            this IEnumerable<TSource> source)
        {
            Check.NotNull(source, nameof(source));

            return new ToAsyncObservable<TSource>(source);
        }
    }
}
