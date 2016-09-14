using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Reactive.Linq
{
    public static partial class AsyncObserver
    {
        public static IAsyncObserver<T> AsAsyncObserver<T>(
            this IAsyncObserver<T> observer)
        {
            return observer;
        }

        public static IAsyncObserver<T> AsAsyncObserver<T>(
            this IObserver<T> observer)
        {
            Check.NotNull(observer, nameof(observer));

            var asyncObserver = observer as IAsyncObserver<T>;
            if (asyncObserver != null)
            {
                return asyncObserver;
            }

            return new AsAsyncObserver<T>(observer);
        }

        public static IAsyncObserver<T> Create<T>(
            Func<T, CancellationToken, Task> onNextAsync)
        {
            Check.NotNull(onNextAsync, nameof(onNextAsync));

            return new AnonymousAsyncObserver<T>(onNextAsync);
        }

        public static IAsyncObserver<T> Create<T>(
            Func<T, CancellationToken, Task> onNextAsync,
            Action<Exception> onError)
        {
            Check.NotNull(onNextAsync, nameof(onNextAsync));
            Check.NotNull(onError, nameof(onError));

            return new AnonymousAsyncObserver<T>(onNextAsync, onError);
        }

        public static IAsyncObserver<T> Create<T>(
            Func<T, CancellationToken, Task> onNextAsync,
            Action onCompleted)
        {
            Check.NotNull(onNextAsync, nameof(onNextAsync));
            Check.NotNull(onCompleted, nameof(onCompleted));

            return new AnonymousAsyncObserver<T>(onNextAsync, null, onCompleted);
        }

        public static IAsyncObserver<T> Create<T>(
            Func<T, CancellationToken, Task> onNextAsync,
            Action<Exception> onError,
            Action onCompleted)
        {
            Check.NotNull(onNextAsync, nameof(onNextAsync));
            Check.NotNull(onError, nameof(onError));
            Check.NotNull(onCompleted, nameof(onCompleted));

            return new AnonymousAsyncObserver<T>(onNextAsync, onError, onCompleted);
        }
    }
}
