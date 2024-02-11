namespace Enterprise.Core.Reactive.Linq
{
    partial class AsyncObservable
    {
        public static IAsyncObservable<TResult> Never<TResult>()
        {
            return Implementations.Never<TResult>.Instance;
        }
    }
}
