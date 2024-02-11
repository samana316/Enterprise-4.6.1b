using System;
using Enterprise.Core.Reactive.Linq.Implementations;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Reactive.Linq
{
    partial class AsyncObservable
    {
        public static IAsyncObservable<TResult> Select<TSource, TResult>(
           this IAsyncObservable<TSource> source,
           Func<TSource, TResult> selector)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));

            return new Select<TSource, TResult>(source, selector);
        }

        public static IAsyncObservable<TResult> Select<TSource, TResult>(
            this IAsyncObservable<TSource> source,
            Func<TSource, int, TResult> selector)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));

            return new Select<TSource, TResult>(source, selector);
        }
    }
}
