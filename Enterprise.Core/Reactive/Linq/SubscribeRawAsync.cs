using System.Threading;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Reactive.Linq
{
    partial class AsyncObservable 
    {
        internal static IAsyncSubscription SubscribeRawAsync<TSource>(
            this IAsyncObservable<TSource> source,
            IAsyncObserver<TSource> observer,
            CancellationToken cancellationToken)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(observer, nameof(observer));
            cancellationToken.ThrowIfCancellationRequested();

            var subjectBase = source as Subjects.AsyncSubjectBase<TSource>;
            if (subjectBase != null)
            {
                var task = subjectBase.SubscribeCoreAsync(observer, cancellationToken);
                return new AsyncSubscription(task, cancellationToken);
            }

            var observableBase = source as AsyncObservableBase<TSource>;
            if (observableBase != null)
            {
                var task = observableBase.SubscribeCoreAsync(observer, cancellationToken);
                return new AsyncSubscription(task, cancellationToken);
            }

            return source.SubscribeAsync(observer, cancellationToken);
        }
    }
}
