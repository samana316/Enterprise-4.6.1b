using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;

namespace Enterprise.Core.Reactive.Linq.Implementations
{
    internal sealed class Do<TSource> : AsyncObservableBase<TSource>
    {
        private readonly IAsyncObservable<TSource> source;

        private readonly IAsyncObserver<TSource> observer;

        public Do(
            IAsyncObservable<TSource> source, 
            IAsyncObserver<TSource> observer)
        {
            this.source = source;
            this.observer = observer;
        }

        public override AsyncIterator<TSource> Clone()
        {
            return new Do<TSource>(this.source, this.observer);
        }

        protected override async Task ProduceAsync(
            IAsyncYield<TSource> yield, 
            CancellationToken cancellationToken)
        {
            try
            {
                await this.source.ForEachAsync(async (item, cancellationToken2) =>
                {
                    await this.observer.OnNextAsync(item, cancellationToken2);
                    await yield.ReturnAsync(item, cancellationToken2);
                }, cancellationToken);
            }
            catch (Exception error)
            {
                this.observer.OnError(error);
            }
            finally
            {
                this.observer.OnCompleted();
            }
        }
    }
}
