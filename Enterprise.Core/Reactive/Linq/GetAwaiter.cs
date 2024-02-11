using System.Runtime.CompilerServices;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Reactive.Linq
{
    partial class AsyncObservable
    {
        public static TaskAwaiter<TSource> GetAwaiter<TSource>(
            this IAsyncObservable<TSource> source)
        {
            Check.NotNull(source, nameof(source));

            var task = source.ToTask();

            return task.GetAwaiter();
        }
    }
}