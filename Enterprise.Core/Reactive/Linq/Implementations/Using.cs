using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;

namespace Enterprise.Core.Reactive.Linq.Implementations
{
    internal sealed class Using<TResult, TResource> : AsyncObservableBase<TResult>
        where TResource : IDisposable
    {
        private readonly Func<TResource> resourceFactory;

        private readonly Func<TResource, IAsyncObservable<TResult>> observableFactory;

        private readonly Func<TResource, CancellationToken, Task<IAsyncObservable<TResult>>> observableFactoryAsync;

        public Using(
            Func<TResource> resourceFactory, 
            Func<TResource, IAsyncObservable<TResult>> observableFactory)
        {
            this.resourceFactory = resourceFactory;
            this.observableFactory = observableFactory;
        }

        public Using(
            Func<TResource> resourceFactory, 
            Func<TResource, CancellationToken, Task<IAsyncObservable<TResult>>> observableFactoryAsync)
        {
            this.resourceFactory = resourceFactory;
            this.observableFactoryAsync = observableFactoryAsync;
        }

        public override AsyncIterator<TResult> Clone()
        {
            if (this.observableFactoryAsync != null)
            {
                return new Using<TResult, TResource>(this.resourceFactory, this.observableFactoryAsync);
            }

            return new Using<TResult, TResource>(this.resourceFactory, this.observableFactory);
        }

        protected override Task ProduceAsync(
            IAsyncYield<TResult> yield, 
            CancellationToken cancellationToken)
        {
            if (this.observableFactoryAsync != null)
            {
                return this.UsingImplAAsync(yield, cancellationToken);
            }

            return this.UsingImplAsync(yield, cancellationToken);
        }

        private async Task UsingImplAsync(
            IAsyncYield<TResult> yield,
            CancellationToken cancellationToken)
        {
            using (var resource = this.resourceFactory())
            {
                var observable = this.observableFactory(resource);

                await observable.ForEachAsync(yield.ReturnAsync, cancellationToken);

                yield.Break();
            }
        }

        private async Task UsingImplAAsync(
           IAsyncYield<TResult> yield,
           CancellationToken cancellationToken)
        {
            using (var resource = this.resourceFactory())
            {
                var observable = await this.observableFactoryAsync(resource, cancellationToken);

                await observable.ForEachAsync(yield.ReturnAsync, cancellationToken);

                yield.Break();
            }
        }
    }
}
