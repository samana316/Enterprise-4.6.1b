using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Common.Runtime.CompilerServices;
using Enterprise.Core.Common.Runtime.ExceptionServices;

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
            var impl = new Impl(this);
            var subscription = this.source.SubscribeRawAsync(impl, this.cancellationToken);

            return subscription.GetAwaiter();
        }

        private sealed class Impl : IAsyncObserver<TSource>
        {
            private readonly ForEachAsync<TSource> parent;

            public Impl(
                ForEachAsync<TSource> parent)
            {
                this.parent = parent;
            }

            public void OnCompleted()
            {
            }

            public void OnError(
                Exception error)
            {
                error.Rethrow();
            }

            public void OnNext(
                TSource value)
            {
                throw new NotImplementedException();
            }

            public Task OnNextAsync(
                TSource value, 
                CancellationToken cancellationToken)
            {
                return parent.onNextAsync(value, parent.cancellationToken);
            }
        }
    }
}
