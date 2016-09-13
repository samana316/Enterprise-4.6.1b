using System;
using Enterprise.Core.Reactive.Linq.Implementations;
using Enterprise.Core.Resources;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Reactive.Linq
{
    partial class AsyncObservable
    {
        public static IAsyncObservable<TSource> Delay<TSource>(
            this IAsyncObservable<TSource> source,
            TimeSpan dueTime)
        {
            Check.NotNull(source, nameof(source));

            if (dueTime < TimeSpan.Zero)
            {
                throw Error.ArgumentOutOfRange(nameof(dueTime));
            }

            return new Delay<TSource>(source, dueTime);
        }

        public static IAsyncObservable<TSource> Delay<TSource>(
            this IAsyncObservable<TSource> source,
            DateTimeOffset dueTime)
        {
            Check.NotNull(source, nameof(source));

            return new Delay<TSource>(source, dueTime);
        }
    }
}
