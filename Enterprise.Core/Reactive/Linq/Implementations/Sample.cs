using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;

namespace Enterprise.Core.Reactive.Linq.Implementations
{
    internal sealed class Sample<TSource> : AsyncObservableBase<TSource>
    {
        private readonly IAsyncObservable<TSource> source;

        private readonly TimeSpan interval;

        public Sample(
            IAsyncObservable<TSource> source, 
            TimeSpan interval)
        {
            this.interval = interval;
            this.source = source;
        }

        public override AsyncIterator<TSource> Clone()
        {
            return new Sample<TSource>(this.source, this.interval);
        }

        protected override Task ProduceAsync(
            IAsyncYield<TSource> yield, 
            CancellationToken cancellationToken)
        {
            var timestamp = DateTimeOffset.Now;

            return this.source.ForEachAsync(async (item, cancellationToken2) =>
            {
                if (DateTimeOffset.Now - timestamp >= this.interval)
                {
                    await yield.ReturnAsync(item, cancellationToken2);
                    timestamp = DateTimeOffset.Now;
                }
            }, cancellationToken);
        }
    }

    internal sealed class Sample<TSource, TSample> : AsyncObservableBase<TSource>
    {
        private readonly IAsyncObservable<TSource> source;

        private readonly IAsyncObservable<TSample> sampler;

        public Sample(
            IAsyncObservable<TSource> source, 
            IObservable<TSample> sampler)
        {
            this.source = source;
            this.sampler = sampler.AsAsyncObservable();
        }

        public override AsyncIterator<TSource> Clone()
        {
            return new Sample<TSource, TSample>(this.source, this.sampler);
        }

        protected override Task ProduceAsync(
            IAsyncYield<TSource> yield, 
            CancellationToken cancellationToken)
        {
            var sample = false;
            var stop = false;

            var samplerTask = this.sampler.ForEachAsync(async (item, cancellationToken2) =>
            {
                if (stop)
                {
                    yield.Break();
                }

                if (!sample)
                {
                    sample = true;
                }

                await Task.Yield();
            }, cancellationToken);

            var sourceTask = this.source.ForEachAsync(async (item, cancellationToken2) =>
            {
                if (sample)
                {
                    await yield.ReturnAsync(item, cancellationToken2);
                    sample = false;
                }
            }, cancellationToken).ContinueWith(t => stop = true);

            return Task.WhenAll(samplerTask, sourceTask);
        }
    }
}
