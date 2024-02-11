using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Enterprise.Core.Reactive.Linq.Implementations;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Reactive.Linq
{
    partial class AsyncObservable
    {
        public static IAsyncObservable<TSource> ToAsyncObservable<TSource>(
            this IEnumerable<TSource> source)
        {
            Check.NotNull(source, nameof(source));

            var asyncObservable = source as IAsyncObservable<TSource>;
            if (asyncObservable != null)
            {
                return asyncObservable;
            }

            var observable = source as IObservable<TSource>;
            if (observable != null)
            {
                return observable.AsAsyncObservable();
            }

            return new ToAsyncObservable<TSource>(source);
        }

        public static IAsyncObservable<TSource> ToAsyncObservable<TSource>(
            this Task<TSource> task)
        {
            Check.NotNull(task, nameof(task));

            return Create<TSource>(async (yield, cancellationToken) => 
            {
                var result = await task;

                await yield.ReturnAsync(result, cancellationToken);
            });
        }
    }
}
