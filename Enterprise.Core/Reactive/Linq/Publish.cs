using Enterprise.Core.Utilities;

namespace Enterprise.Core.Reactive.Linq
{
    partial class AsyncObservable
    {
        public static IConnectableAsyncObservable<TSource> Publish<TSource>(
            this IAsyncObservable<TSource> source)
        {
            Check.NotNull(source, nameof(source));

            return new ConnectableAsyncObservable<TSource>(source);
        }
    }
}
