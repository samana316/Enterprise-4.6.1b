using Enterprise.Core.Reactive.Linq.Implementations;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Reactive.Linq
{
    partial class AsyncObservable
    {
        public static IAsyncObservable<TSource> Retry<TSource>(
            this IAsyncObservable<TSource> source)
        {
            Check.NotNull(source);

            return new Retry<TSource>(source);
        }

        public static IAsyncObservable<TSource> Retry<TSource>(
            this IAsyncObservable<TSource> source, 
            int retryCount)
        {
            Check.NotNull(source);
            Check.NotLessThanDefault(retryCount);

            if (retryCount == 0)
            {
                return Empty<TSource>();
            }

            if (retryCount == 1)
            {
                return source;
            }

            return new Retry<TSource>(source, retryCount);
        }
    }
}
