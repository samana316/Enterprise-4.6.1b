using System;
using Enterprise.Core.Utilities;
using Enterprise.Core.Reactive.Linq.Implementations;
using System.Threading;
using System.Threading.Tasks;

namespace Enterprise.Core.Reactive.Linq
{
    partial class AsyncObservable
    {
        public static IAsyncObservable<TResult> Using<TResult, TResource>(
            Func<TResource> resourceFactory,
            Func<TResource, IAsyncObservable<TResult>> observableFactory)
            where TResource : IDisposable
        {
            Check.NotNull(resourceFactory, nameof(resourceFactory));
            Check.NotNull(observableFactory, nameof(observableFactory));

            return new Using<TResult, TResource>(resourceFactory, observableFactory);
        }

        public static IAsyncObservable<TResult> Using<TResult, TResource>(
            Func<TResource> resourceFactory,
            Func<TResource, CancellationToken, Task<IAsyncObservable<TResult>>> observableFactoryAsync)
            where TResource : IDisposable
        {
            Check.NotNull(resourceFactory, nameof(resourceFactory));
            Check.NotNull(observableFactoryAsync, nameof(observableFactoryAsync));

            return new Using<TResult, TResource>(resourceFactory, observableFactoryAsync);
        }
    }
}