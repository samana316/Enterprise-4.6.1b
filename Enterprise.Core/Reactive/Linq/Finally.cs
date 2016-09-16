using System;
using Enterprise.Core.Reactive.Linq.Implementations;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Reactive.Linq
{
    partial class AsyncObservable
    {
        public static IAsyncObservable<TSource> Finally<TSource>(
            this IAsyncObservable<TSource> source,
            Action finallyAction)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(finallyAction, nameof(finallyAction));

            return new Finally<TSource>(source, finallyAction);
        }
    }
}
