﻿namespace Enterprise.Core.Reactive.Linq
{
    partial class AsyncObservable
    {
        public static IAsyncObservable<TResult> Empty<TResult>()
        {
            return Implementations.Empty<TResult>.Instance;
        }

        public static IAsyncObservable<TResult> Empty<TResult>(
            TResult witness)
        {
            return Empty<TResult>();
        }
    }
}
