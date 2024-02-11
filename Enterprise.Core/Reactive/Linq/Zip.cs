using System;
using System.Collections.Generic;
using System.Linq;
using Enterprise.Core.Reactive.Linq.Implementations;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Reactive.Linq
{
    partial class AsyncObservable
    {
        public static IAsyncObservable<IList<TSource>> Zip<TSource>(
            params IAsyncObservable<TSource>[] sources)
        {
            return sources.AsEnumerable().Zip();
        }

        public static IAsyncObservable<IList<TSource>> Zip<TSource>(
            this IEnumerable<IAsyncObservable<TSource>> sources)
        {
            Check.NotNull(sources, nameof(sources));

            return new Zip<TSource>(sources);
        }

        public static IAsyncObservable<TResult> Zip<TSource, TResult>(
            this IEnumerable<IAsyncObservable<TSource>> sources, 
            Func<IList<TSource>, TResult> resultSelector)
        {
            Check.NotNull(sources, nameof(sources));
            Check.NotNull(resultSelector, nameof(resultSelector));

            return sources.Zip().Select(resultSelector);
        }

        public static IAsyncObservable<TResult> Zip<TFirst, TSecond, TResult>(
            this IAsyncObservable<TFirst> first,
            IObservable<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector)
        {
            Check.NotNull(first, nameof(first));
            Check.NotNull(second, nameof(second));
            Check.NotNull(resultSelector, nameof(resultSelector));

            return new Zip<TFirst, TSecond, TResult>(first, second, resultSelector);
        }

        public static IAsyncObservable<TResult> Zip<TFirst, TSecond, TResult>(
            this IAsyncObservable<TFirst> first,
            IEnumerable<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector)
        {
            Check.NotNull(first, nameof(first));
            Check.NotNull(second, nameof(second));
            Check.NotNull(resultSelector, nameof(resultSelector));

            return new Zip<TFirst, TSecond, TResult>(first, second, resultSelector);
        }

        public static IAsyncObservable<TResult> Zip<TSource1, TSource2, TSource3, TResult>(
            this IAsyncObservable<TSource1> source1,
            IObservable<TSource2> source2, 
            IObservable<TSource3> source3, 
            Func<TSource1, TSource2, TSource3, TResult> resultSelector)
        {
            Check.NotNull(source1, nameof(source1));
            Check.NotNull(source2, nameof(source2));
            Check.NotNull(source3, nameof(source3));
            Check.NotNull(resultSelector, nameof(resultSelector));

            return source1
                .Zip(source2, (x1, x2) => new { x1, x2 })
                .Zip(source3, (a, x3) => resultSelector(a.x1, a.x2, x3));
        }

        public static IAsyncObservable<TResult> Zip<TSource1, TSource2, TSource3, TSource4, TResult>(
            this IAsyncObservable<TSource1> source1,
            IObservable<TSource2> source2,
            IObservable<TSource3> source3,
            IObservable<TSource4> source4,
            Func<TSource1, TSource2, TSource3, TSource4, TResult> resultSelector)
        {
            Check.NotNull(source1, nameof(source1));
            Check.NotNull(source2, nameof(source2));
            Check.NotNull(source3, nameof(source3));
            Check.NotNull(source4, nameof(source4));
            Check.NotNull(resultSelector, nameof(resultSelector));

            return source1
                .Zip(source2, source3, (x1, x2, x3) => new { x1, x2, x3 })
                .Zip(source4, (a, x4) => resultSelector(a.x1, a.x2, a.x3, x4));
        }
    }
}
