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
        public static IAsyncObservable<TSource> Concat<TSource>(
            this IAsyncObservable<TSource> first,
            IAsyncObservable<TSource> second)
        {
            Check.NotNull(first, nameof(first));
            Check.NotNull(second, nameof(second));

            return new Concat<TSource>(new[] { first, second });
        }

        public static IAsyncObservable<TSource> Concat<TSource>(
            this IAsyncObservable<TSource> first,
            IObservable<TSource> second)
        {
            return first.Concat(second.AsAsyncObservable());
        }

        public static IAsyncObservable<TSource> Concat<TSource>(
            this IAsyncObservable<TSource> first,
            IEnumerable<TSource> second)
        {
            return first.Concat(second.ToAsyncObservable());
        }

        public static IAsyncObservable<TSource> Concat<TSource>(
            params IAsyncObservable<TSource>[] sources)
        {
            return sources.AsEnumerable().Concat();
        }

        public static IAsyncObservable<TSource> Concat<TSource>(
            this IAsyncObservable<TSource> first,
            params IAsyncObservable<TSource>[] sources)
        {
            Check.NotNull(first, nameof(first));
            Check.NotNull(sources, nameof(sources));

            return first.Concat(sources.Concat());
        }

        public static IAsyncObservable<TSource> Concat<TSource>(
            this IAsyncObservable<TSource> first,
            params IObservable<TSource>[] sources)
        {
            Check.NotNull(first, nameof(first));
            Check.NotNull(sources, nameof(sources));

            return first.Concat(sources.Select(AsAsyncObservable).Concat());
        }

        public static IAsyncObservable<TSource> Concat<TSource>(
            this IAsyncObservable<TSource> first,
            params IEnumerable<TSource>[] sources)
        {
            Check.NotNull(first, nameof(first));
            Check.NotNull(sources, nameof(sources));

            return first.Concat(sources.Select(ToAsyncObservable).Concat());
        }

        public static IAsyncObservable<TSource> Concat<TSource>(
            this IAsyncObservable<IAsyncObservable<TSource>> sources)
        {
            Check.NotNull(sources, nameof(sources));

            return new Concat<TSource>(sources);
        }

        public static IAsyncObservable<TSource> Concat<TSource>(
            this IEnumerable<IAsyncObservable<TSource>> sources)
        {
            Check.NotNull(sources, nameof(sources));

            return new Concat<TSource>(sources);
        }

        public static IAsyncObservable<TSource> Concat<TSource>(
            this IObservable<IAsyncObservable<TSource>> sources)
        {
            Check.NotNull(sources, nameof(sources));

            return new Concat<TSource>(sources);
        }

        public static IAsyncObservable<TSource> Concat<TSource>(
            this IAsyncObservable<Task<TSource>> sources)
        {
            Check.NotNull(sources, nameof(sources));

            return Create<TSource>((yield, cancellationToken) =>
            {
                return sources.ForEachAsync(async (item, cancellationToken2) =>
                {
                    await yield.ReturnAsync(await item, cancellationToken2);
                }, cancellationToken);
            });
        }
    }
}
