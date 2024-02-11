using System;
using Enterprise.Core.Reactive.Linq.Implementations;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Reactive.Linq
{
    partial class AsyncObservable
    {
        public static IAsyncObservable<TSource> Sample<TSource>(
            this IAsyncObservable<TSource> source,
            TimeSpan interval)
        {
            Check.NotNull(source, nameof(source));
            Check.NotLessThanDefault(interval, nameof(interval));

            return new Sample<TSource>(source, interval);
        }

        public static IAsyncObservable<TSource> Sample<TSource, TSample>(
            this IAsyncObservable<TSource> source,
            IAsyncObservable<TSample> sampler)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(sampler, nameof(sampler));

            return new Sample<TSource, TSample>(source, sampler);
        }
    }
}
