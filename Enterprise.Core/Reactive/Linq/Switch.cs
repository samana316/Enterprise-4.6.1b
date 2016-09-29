using System;
using System.Threading.Tasks;
using Enterprise.Core.Reactive.Linq.Implementations;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Reactive.Linq
{
    partial class AsyncObservable
    {
        public static IAsyncObservable<TSource> Switch<TSource>(
            this IObservable<IAsyncObservable<TSource>> sources)
        {
            Check.NotNull(sources, nameof(sources));

            return new Switch<TSource>(sources);
        }

        public static IAsyncObservable<TSource> Switch<TSource>(
           this IAsyncObservable<Task<TSource>> sources)
        {
            Check.NotNull(sources, nameof(sources));

            return new Switch<TSource>(sources);
        }
    }
}
