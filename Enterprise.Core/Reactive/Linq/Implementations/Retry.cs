using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;

namespace Enterprise.Core.Reactive.Linq.Implementations
{
    internal sealed class Retry<TSource> : AsyncObservableBase<TSource>
    {
        private readonly IAsyncObservable<TSource> source;

        private readonly int? retryCount;

        public Retry(
            IAsyncObservable<TSource> source, 
            int? retryCount = null)
        {
            this.source = source;
            this.retryCount = retryCount;
        }

        public override AsyncIterator<TSource> Clone()
        {
            return new Retry<TSource>(this.source, this.retryCount);
        }

        protected override Task ProduceAsync(
            IAsyncYield<TSource> yield, 
            CancellationToken cancellationToken)
        {
            if (this.retryCount.HasValue)
            {
                return RetryCountImplAsync(yield, cancellationToken);
            }

            return this.RetryImplAsync(yield, cancellationToken);
        }

        private async Task RetryImplAsync(
            IAsyncYield<TSource> yield, 
            CancellationToken cancellationToken)
        {
            var terminate = false;

            while (true)
            {
                if (terminate)
                {
                    yield.Break();
                }

                try
                {
                    await yield.ReturnAllAsync(this.source, cancellationToken);
                    terminate = true;
                }
                catch
                {
                    terminate = false;
                }
            }
        }

        private async Task RetryCountImplAsync(
            IAsyncYield<TSource> yield,
            CancellationToken cancellationToken)
        {
            var terminate = false;

            for (var i = 0; i < this.retryCount.Value; i++)
            {
                if (terminate)
                {
                    yield.Break();
                }

                if (i == this.retryCount.Value - 1)
                {
                    await yield.ReturnAllAsync(this.source, cancellationToken);
                }
                else
                {
                    try
                    {
                        await yield.ReturnAllAsync(this.source, cancellationToken);
                        terminate = true;
                    }
                    catch
                    {
                        terminate = false;
                    }
                }
            }
        }
    }
}
