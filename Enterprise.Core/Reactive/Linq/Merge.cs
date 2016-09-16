using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Enterprise.Core.Reactive.Linq.Implementations;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Reactive.Linq
{
    partial class AsyncObservable
    {
        public static IAsyncObservable<TSource> Merge<TSource>(
            this IAsyncObservable<TSource> first,
            IAsyncObservable<TSource> second)
        {
            Check.NotNull(first, nameof(first));
            Check.NotNull(second, nameof(second));

            return new Merge<TSource>(new[] { first, second });
        }

        public static IAsyncObservable<TSource> Merge<TSource>(
            this IAsyncObservable<TSource> first,
            IObservable<TSource> second)
        {
            return first.Merge(second.AsAsyncObservable());
        }

        public static IAsyncObservable<TSource> Merge<TSource>(
            this IAsyncObservable<TSource> first,
            IEnumerable<TSource> second)
        {
            return first.Merge(second.ToAsyncObservable());
        }

        public static IAsyncObservable<TSource> Merge<TSource>(
            params IAsyncObservable<TSource>[] sources)
        {
            return sources.AsEnumerable().Merge();
        }

        public static IAsyncObservable<TSource> Merge<TSource>(
            this IAsyncObservable<TSource> first,
            params IAsyncObservable<TSource>[] sources)
        {
            Check.NotNull(first, nameof(first));
            Check.NotNull(sources, nameof(sources));

            return first.Merge(sources.Merge());
        }

        public static IAsyncObservable<TSource> Merge<TSource>(
            this IAsyncObservable<TSource> first,
            params IObservable<TSource>[] sources)
        {
            Check.NotNull(first, nameof(first));
            Check.NotNull(sources, nameof(sources));

            return first.Merge(sources.Select(AsAsyncObservable).Merge());
        }

        public static IAsyncObservable<TSource> Merge<TSource>(
            this IAsyncObservable<TSource> first,
            params IEnumerable<TSource>[] sources)
        {
            Check.NotNull(first, nameof(first));
            Check.NotNull(sources, nameof(sources));

            return first.Merge(sources.Select(ToAsyncObservable).Merge());
        }

        public static IAsyncObservable<TSource> Merge<TSource>(
            this IAsyncObservable<IAsyncObservable<TSource>> sources)
        {
            Check.NotNull(sources, nameof(sources));

            return new Merge<TSource>(sources);
        }

        public static IAsyncObservable<TSource> Merge<TSource>(
            this IEnumerable<IAsyncObservable<TSource>> sources)
        {
            Check.NotNull(sources, nameof(sources));

            return new Merge<TSource>(sources);
        }

        public static IAsyncObservable<TSource> Merge<TSource>(
            this IObservable<IAsyncObservable<TSource>> sources)
        {
            Check.NotNull(sources, nameof(sources));

            return new Merge<TSource>(sources);
        }

        public static IAsyncObservable<TSource> Merge<TSource>(
            this IAsyncObservable<Task<TSource>> sources)
        {
            Check.NotNull(sources, nameof(sources));

            return new Merge<TSource>(sources);
        }
    }
}
