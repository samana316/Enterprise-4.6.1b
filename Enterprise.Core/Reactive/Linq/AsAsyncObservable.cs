﻿using System;
using Enterprise.Core.Reactive.Linq.Implementations;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Reactive.Linq
{
    partial class AsyncObservable
    {
        public static IAsyncObservable<TSource> AsAsyncObservable<TSource>(
            this IAsyncObservable<TSource> source)
        {
            return source;
        }

        public static IAsyncObservable<TSource> AsAsyncObservable<TSource>(
            this IObservable<TSource> source)
        {
            Check.NotNull(source, nameof(source));

            return new AsAsyncObservable<TSource>(source);
        }
    }
}
