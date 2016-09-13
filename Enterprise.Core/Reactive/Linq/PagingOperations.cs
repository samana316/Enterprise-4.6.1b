using System;
using Enterprise.Core.Reactive.Linq.Implementations;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Reactive.Linq
{
    partial class AsyncObservable
    {
        public static IAsyncObservable<TSource> Skip<TSource>(
            this IAsyncObservable<TSource> source,
            int count)
        {
            Check.NotNull(source, nameof(source));

            return new Skip<TSource>(source, count);
        }

        public static IAsyncObservable<TSource> SkipWhile<TSource>(
            this IAsyncObservable<TSource> source,
            Func<TSource, bool> predicate)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(predicate, nameof(predicate));

            return new SkipWhile<TSource>(source, predicate);
        }

        public static IAsyncObservable<TSource> SkipWhile<TSource>(
            this IAsyncObservable<TSource> source,
            Func<TSource, int, bool> predicate)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(predicate, nameof(predicate));

            return new SkipWhile<TSource>(source, predicate);
        }

        public static IAsyncObservable<TSource> Take<TSource>(
            this IAsyncObservable<TSource> source,
            int count)
        {
            Check.NotNull(source, nameof(source));

            return new Take<TSource>(source, count);
        }

        public static IAsyncObservable<TSource> TakeWhile<TSource>(
            this IAsyncObservable<TSource> source,
            Func<TSource, bool> predicate)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(predicate, nameof(predicate));

            return new TakeWhile<TSource>(source, predicate);
        }

        public static IAsyncObservable<TSource> TakeWhile<TSource>(
            this IAsyncObservable<TSource> source,
            Func<TSource, int, bool> predicate)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(predicate, nameof(predicate));

            return new TakeWhile<TSource>(source, predicate);
        }
    }
}
