using System.Collections;
using System.Collections.Generic;
using Enterprise.Core.Linq;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Reactive.Linq
{
    using Implementations;

    partial class AsyncObservable
    {
        public static IAsyncEnumerable<TSource> AsAsyncEnumerable<TSource>(
            this IAsyncObservable<TSource> source)
        {
            if (source == null)
            {
                return null;
            }

            var enumerable = source as IEnumerable;
            if (enumerable != null)
            {
                IAsyncEnumerable<TSource> asyncEnumerable;
                if (enumerable.TryAsAsyncEnumerable(out asyncEnumerable))
                {
                    return asyncEnumerable;
                }
            }

            return new AsAsyncEnumerable<TSource>(source);
        }

        public static IEnumerable<TSource> AsEnumerable<TSource>(
            this IAsyncObservable<TSource> source)
        {
            return source.AsAsyncEnumerable();
        }

        public static IAsyncEnumerator<TSource> GetAsyncEnumerator<TSource>(
            this IAsyncObservable<TSource> source)
        {
            Check.NotNull(source, nameof(source));

            return source.AsAsyncEnumerable().GetAsyncEnumerator();
        }

        public static IEnumerator<TSource> GetEnumerator<TSource>(
           this IAsyncObservable<TSource> source)
        {
            Check.NotNull(source, nameof(source));

            return source.AsEnumerable().GetEnumerator();
        }
    }
}
