using Enterprise.Core.Reactive.Linq.Implementations;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Reactive.Linq
{
    partial class AsyncObservable
    {
        public static IAsyncObservable<TResult> Repeat<TResult>(
            TResult element)
        {
            return new Repeat<TResult>(element, null);
        }

        public static IAsyncObservable<TResult> Repeat<TResult>(
            TResult element,
            int count)
        {
            Check.NotLessThanDefault(count, nameof(count));

            if (count == 0)
            {
                return Empty<TResult>();
            }

            if (count == 1)
            {
                return Return(element);
            }

            return new Repeat<TResult>(element, count);
        }

        public static IAsyncObservable<TSource> Repeat<TSource>(
           this IAsyncObservable<TSource> source)
        {
            Check.NotNull(source, nameof(source));

            return new Repeat<TSource>(source, null);
        }

        public static IAsyncObservable<TSource> Repeat<TSource>(
            this IAsyncObservable<TSource> source,
            int count)
        {
            Check.NotNull(source, nameof(source));
            Check.NotLessThanDefault(count, nameof(count));

            if (count == 0)
            {
                return Empty<TSource>();
            }

            if (count == 1)
            {
                return source;
            }

            return new Repeat<TSource>(source, count);
        }
    }
}
