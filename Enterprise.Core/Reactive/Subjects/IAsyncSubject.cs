namespace Enterprise.Core.Reactive.Subjects
{
    public interface IAsyncSubject<T> : IAsyncSubject<T, T>
    {
    }

    public interface IAsyncSubject<in TSource, out TResult> :
        IAsyncObserver<TSource>, IAsyncObservable<TResult>
    {
    }
}
