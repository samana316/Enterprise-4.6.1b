using System.Collections.Generic;
using System.Linq;
using Enterprise.Core.Reactive.Linq.Implementations;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Reactive.Linq
{
    partial class AsyncObservable
    {
        public static IAsyncObservable<TSource> StartWith<TSource>(
            this IAsyncObservable<TSource> source,
            params TSource[] values)
        {
            return source.StartWith(values.AsEnumerable());
        }

        public static IAsyncObservable<TSource> StartWith<TSource>(
            this IAsyncObservable<TSource> source,
            IEnumerable<TSource> values)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(values, nameof(values));

            return new StartWith<TSource>(source, values);
        }
    }
}
