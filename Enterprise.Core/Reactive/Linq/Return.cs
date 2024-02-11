using Enterprise.Core.Reactive.Linq.Implementations;

namespace Enterprise.Core.Reactive.Linq
{
    partial class AsyncObservable
    {
        public static IAsyncObservable<TResult> Return<TResult>(
            TResult value)
        {
            return new Return<TResult>(value);
        }
    }
}
