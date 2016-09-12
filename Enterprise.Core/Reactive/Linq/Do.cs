using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Reactive.Linq.Implementations;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Reactive.Linq
{
    partial class AsyncObservable
    {
        public static IAsyncObservable<TSource> Do<TSource>(
            this IAsyncObservable<TSource> source,
            IAsyncObserver<TSource> observer)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(observer, nameof(observer));

            return new Do<TSource>(source, observer);
        }

        public static IAsyncObservable<TSource> Do<TSource>(
            this IAsyncObservable<TSource> source,
            Func<TSource, CancellationToken, Task> onNextAsync)
        {
            var observer = AsyncObserver.Create(onNextAsync);

            return source.Do(observer);
        }

        public static IAsyncObservable<TSource> Do<TSource>(
            this IAsyncObservable<TSource> source,
            Func<TSource, CancellationToken, Task> onNextAsync,
            Action<Exception> onError)
        {
            var observer = AsyncObserver.Create(onNextAsync, onError);

            return source.Do(observer);
        }

        public static IAsyncObservable<TSource> Do<TSource>(
            this IAsyncObservable<TSource> source,
            Func<TSource, CancellationToken, Task> onNextAsync,
            Action onCompleted)
        {
            var observer = AsyncObserver.Create(onNextAsync, onCompleted);

            return source.Do(observer);
        }

        public static IAsyncObservable<TSource> Do<TSource>(
            this IAsyncObservable<TSource> source,
            Func<TSource, CancellationToken, Task> onNextAsync,
            Action<Exception> onError,
            Action onCompleted)
        {
            var observer = AsyncObserver.Create(onNextAsync, onError, onCompleted);

            return source.Do(observer);
        }
    }
}
