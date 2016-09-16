using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;

namespace Enterprise.Core.Reactive.Linq.Implementations
{
    internal sealed class Merge<TSource> : AsyncObservableBase<TSource>
    {
        private readonly IAsyncObservable<IAsyncObservable<TSource>> sources;

        private readonly IAsyncObservable<Task<TSource>> sourcesT;

        public Merge(
            IObservable<IAsyncObservable<TSource>> sources)
        {
            this.sources = sources.AsAsyncObservable();
        }

        public Merge(
            IEnumerable<IAsyncObservable<TSource>> sources)
        {
            this.sources = sources.ToAsyncObservable();
        }

        public Merge(
            IAsyncObservable<Task<TSource>> sources)
        {
            this.sourcesT = sources;
        }

        public override AsyncIterator<TSource> Clone()
        {
            if (this.sources == null)
            {
                return new Merge<TSource>(this.sourcesT);
            }

            return new Merge<TSource>(this.sources);
        }

        protected override Task ProduceAsync(
            IAsyncYield<TSource> yield,
            CancellationToken cancellationToken)
        {
            if (this.sources == null)
            {
                return this.MergeImplTAsync(yield, cancellationToken);
            }

            return this.MergeImplAsync(yield, cancellationToken);
        }

        private async Task MergeImplAsync(
            IAsyncYield<TSource> yield,
            CancellationToken cancellationToken)
        {
            var tasks = new List<Task>();

            await this.sources.ForEachAsync((source, cancellationToken2) =>
            {
                tasks.Add(yield.ReturnAllAsync(source, cancellationToken2));

                return Task.CompletedTask;
            }, cancellationToken);

            await Task.WhenAll(tasks);
        }

        private async Task MergeImplTAsync(
            IAsyncYield<TSource> yield,
            CancellationToken cancellationToken)
        {
            var tasks = new List<Task>();

            await this.sourcesT.ForEachAsync(async (t, cancellationToken2) => 
            {
                await Task.Yield();

                var task = t.ContinueWith(async t2 => 
                {
                    await yield.ReturnAsync(t2.Result, cancellationToken2);
                });

                tasks.Add(task.Unwrap());
            }, cancellationToken);

            await Task.WhenAll(tasks);
        }
    }
}
