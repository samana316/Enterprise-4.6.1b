﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Reactive
{
    public static class AsyncSubject
    {
        public static IAsyncSubject<TSource> AsAsyncSubject<TSource>(
            this IAsyncSubject<TSource> source)
        {
            return source;
        }

        public static IAsyncSubject<TSource> AsAsyncSubject<TSource>(
            this IAsyncObservable<TSource> source)
        {
            Check.NotNull(source, nameof(source));

            return new AsAsyncSubject<TSource>(source);
        }

        public static IAsyncSubject<T> Create<T>(
            Func<IAsyncYield<T>, CancellationToken, Task> producer)
        {
            Check.NotNull(producer, nameof(producer));

            return new AnonymousAsyncSubject<T>(producer);
        }
    }
}
