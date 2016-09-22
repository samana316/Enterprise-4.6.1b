using System;
using System.Collections.Generic;
using Enterprise.Core.Reactive.Linq.Implementations;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Reactive.Linq
{
    partial class AsyncObservable
    {
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
                .Zip(source3, (x12, x3) => resultSelector(x12.x1, x12.x2, x3));
        }
    }
}
