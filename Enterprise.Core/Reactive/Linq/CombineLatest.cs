using System;
using Enterprise.Core.Reactive.Linq.Implementations;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Reactive.Linq
{
    partial class AsyncObservable
    {
        public static IAsyncObservable<TResult> CombineLatest<TFirst, TSecond, TResult>(
            this IAsyncObservable<TFirst> first,
            IObservable<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector)
        {
            Check.NotNull(first, nameof(first));
            Check.NotNull(second, nameof(second));
            Check.NotNull(resultSelector, nameof(resultSelector));

            return new Zip<TFirst, TSecond, TResult>(first, second, resultSelector);
        }
    }
}
