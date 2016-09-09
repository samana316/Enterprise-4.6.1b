using System;
using Enterprise.Core.Reactive.Linq.Implementations;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Reactive.Linq
{
    partial class AsyncObservable
    {
        public static IAsyncObservable<TResult> Throw<TResult>(
            Exception exception)
        {
            Check.NotNull(exception, nameof(exception));

            return new Throw<TResult>(exception);
        }
    }
}
