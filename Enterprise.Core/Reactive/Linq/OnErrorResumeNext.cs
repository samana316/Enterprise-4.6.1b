using System.Collections.Generic;
using System.Linq;
using Enterprise.Core.Reactive.Linq.Implementations;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Reactive.Linq
{
    partial class AsyncObservable
    {
        public static IAsyncObservable<TSource> OnErrorResumeNext<TSource>(
            this IAsyncObservable<TSource> first,
            IAsyncObservable<TSource> second)
        {
            Check.NotNull(first, nameof(first));
            Check.NotNull(second, nameof(second));

            return new OnErrorResumeNext<TSource>(new[] { first, second });
        }

        public static IAsyncObservable<TSource> OnErrorResumeNext<TSource>(
            params IAsyncObservable<TSource>[] sources)
        {
            return sources.AsEnumerable().OnErrorResumeNext();
        }

        public static IAsyncObservable<TSource> OnErrorResumeNext<TSource>(
            this IEnumerable<IAsyncObservable<TSource>> sources)
        {
            Check.NotNull(sources, nameof(sources));

            return new OnErrorResumeNext<TSource>(sources);
        }
    }
}
