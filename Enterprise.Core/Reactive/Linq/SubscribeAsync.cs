using System;
using System.Threading;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Reactive.Linq
{
    partial class AsyncObservable
    {
        public static IAsyncSubscription SubscribeAsync<TSource>(
            this IAsyncObservable<TSource> source,
            IAsyncObserver<TSource> observer)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(observer, nameof(observer));

            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            var observableBase = source as AsyncObservableBase<TSource>;
            if (observableBase != null)
            {
                var task = observableBase.SubscribeCoreAsync(observer, cancellationToken);
                return new AsyncSubscription(task, cancellationTokenSource);
            }

            return source.SubscribeAsync(observer, cancellationToken);
        }

        public static IAsyncSubscription SubscribeAsync<TSource>(
            this IObservable<TSource> source,
            IAsyncObserver<TSource> observer,
            CancellationToken cancellationToken)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(observer, nameof(observer));

            return source.AsAsyncObservable().SubscribeAsync(observer, cancellationToken);
        }

        public static IAsyncSubscription SubscribeAsync<TSource>(
            this IObservable<TSource> source,
            IAsyncObserver<TSource> observer)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(observer, nameof(observer));

            return source.AsAsyncObservable().SubscribeAsync(observer);
        }
    }
}
