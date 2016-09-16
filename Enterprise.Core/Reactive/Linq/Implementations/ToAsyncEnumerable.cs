using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Common.Runtime.CompilerServices;
using Enterprise.Core.Common.Runtime.ExceptionServices;
using Enterprise.Core.Linq;

namespace Enterprise.Core.Reactive.Linq.Implementations
{
    internal sealed class ToAsyncEnumerable<TSource> : AsyncEnumerableBase<TSource>
    {
        private readonly IAsyncObservable<TSource> source;

        public ToAsyncEnumerable(
            IAsyncObservable<TSource> source)
        {
            this.source = source;
        }

        public override AsyncIterator<TSource> Clone()
        {
            return new ToAsyncEnumerable<TSource>(this.source);
        }

        protected override Task EnumerateAsync(
            IAsyncYield<TSource> yield, 
            CancellationToken cancellationToken)
        {
            var impl = new Impl(yield);

            return this.source.SubscribeAsync(impl, cancellationToken).ToTask();
        }

        private sealed class Impl : IAsyncObserver<TSource>
        {
            private readonly IAsyncYield<TSource> yield;

            public Impl(
                IAsyncYield<TSource> yield)
            {
                this.yield = yield;
            }

            public void OnCompleted()
            {
                this.yield.Break();
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
                return this.yield.ReturnAsync(value, cancellationToken);
            }
        }
    }
}
