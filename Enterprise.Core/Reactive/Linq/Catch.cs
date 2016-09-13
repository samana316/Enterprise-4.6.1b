using System;
using System.Collections.Generic;
using System.Linq;
using Enterprise.Core.Reactive.Linq.Implementations;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Reactive.Linq
{
    partial class AsyncObservable
    {
        public static IAsyncObservable<TSource> Catch<TSource, TException>(
            this IAsyncObservable<TSource> source,
            Func<TException, IAsyncObservable<TSource>> handler)
            where TException : Exception
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(handler, nameof(handler));

            return new Catch<TSource, TException>(source, handler);
        }

        public static IAsyncObservable<TSource> Catch<TSource>(
            this IAsyncObservable<TSource> first,
            IAsyncObservable<TSource> second)
        {
            Check.NotNull(first, nameof(first));
            Check.NotNull(second, nameof(second));

            return new Catch<TSource>(new[] { first, second });
        }

        public static IAsyncObservable<TSource> Catch<TSource>(
            params IAsyncObservable<TSource>[] sources)
        {
            return sources.AsEnumerable().Catch();
        }

        public static IAsyncObservable<TSource> Catch<TSource>(
            this IEnumerable<IAsyncObservable<TSource>> sources)
        {
            Check.NotNull(sources, nameof(sources));

            return new Catch<TSource>(sources);
        }
    }
}
