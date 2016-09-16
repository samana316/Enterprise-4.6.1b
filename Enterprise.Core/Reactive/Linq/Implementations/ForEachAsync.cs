using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Common.Runtime.CompilerServices;

namespace Enterprise.Core.Reactive.Linq.Implementations
{
    internal sealed class ForEachAsync<TSource> : IAwaitable
    {
        private readonly IAsyncObservable<TSource> source;

        private readonly Func<TSource, CancellationToken, Task> onNextAsync;

        private readonly CancellationToken cancellationToken;

        public ForEachAsync(
            IAsyncObservable<TSource> source, 
            Func<TSource, CancellationToken, Task> onNextAsync, 
            CancellationToken cancellationToken)
        {
            this.source = source;
            this.onNextAsync = onNextAsync;
            this.cancellationToken = cancellationToken;
        }

        public IAwaiter GetAwaiter()
        {
            var impl = new ForEachAsyncObserver(this);
            var subscription = this.source.SubscribeRawAsync(impl, this.cancellationToken);

            return subscription.GetAwaiter();
        }

        private sealed class ForEachAsyncObserver : AsyncObserverBase<TSource>
        {
            private readonly ForEachAsync<TSource> parent;

            public ForEachAsyncObserver(
                ForEachAsync<TSource> parent)
            {
                this.parent = parent;
            }

            public override Task OnNextAsync(
                TSource value, 
                CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();

                return parent.onNextAsync(value, parent.cancellationToken);
            }
        }
    }
}
