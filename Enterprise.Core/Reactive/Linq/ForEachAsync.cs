using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Common.Runtime.CompilerServices;
using Enterprise.Core.Reactive.Linq.Implementations;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Reactive.Linq
{
    partial class AsyncObservable
    {
        public static Task ForEachAsync<TSource>(
            this IAsyncObservable<TSource> source,
            Func<TSource, CancellationToken, Task> onNextAsync,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(source, nameof(source));
            Check.NotNull(onNextAsync, nameof(onNextAsync));

            var observableBase = source as AsyncObservableBase<TSource>;
            if (observableBase != null)
            {
                var observer = AsyncObserver.Create(onNextAsync);
                return observableBase.SubscribeCoreAsync(observer, cancellationToken);
            }

            return new ForEachAsync<TSource>(source, onNextAsync, cancellationToken).ToTask();
        }
    }
}

namespace Enterprise.Core.Reactive.Linq.Implementations
{
    internal static partial class AsyncObservableImpl
    {
        public static Task ForEachAsync<TSource>(
            this IAsyncObservable<TSource> source,
            Func<TSource, CancellationToken, Task> onNextAsync,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(source, nameof(source));
            Check.NotNull(onNextAsync, nameof(onNextAsync));

            var observableBase = source as AsyncObservableBase<TSource>;
            if (observableBase != null)
            {
                var observer = AsyncObserver.Create(onNextAsync);
                return observableBase.UnsafeSubscribeCoreAsync(observer, cancellationToken);
            }

            return new ForEachAsync<TSource>(source, onNextAsync, cancellationToken).ToTask();
        }
    }
}