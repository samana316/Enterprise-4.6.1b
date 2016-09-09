using System.Collections.Generic;
using Enterprise.Core.Reactive.Linq.Implementations;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Reactive.Linq
{
    partial class AsyncObservable
    {
        public static IAsyncObservable<bool> SequenceEqual<TSource>(
            this IAsyncObservable<TSource> first,
            IEnumerable<TSource> second)
        {
            return first.SequenceEqual(second, EqualityComparer<TSource>.Default);
        }

        public static IAsyncObservable<bool> SequenceEqual<TSource>(
            this IAsyncObservable<TSource> first,
            IEnumerable<TSource> second,
            IEqualityComparer<TSource> comparer)
        {
            Check.NotNull(first, nameof(first));
            Check.NotNull(second, nameof(second));

            return new SequenceEqual<TSource>(first, second, comparer);
        }
    }
}