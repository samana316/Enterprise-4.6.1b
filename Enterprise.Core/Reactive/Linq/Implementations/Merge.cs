using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;

namespace Enterprise.Core.Reactive.Linq.Implementations
{
    internal sealed class Merge<TSource> : AsyncObservableBase<TSource>
    {
        private readonly IAsyncObservable<IAsyncObservable<TSource>> sources;

        private readonly int maxConcurrent;

        private readonly IAsyncObservable<Task<TSource>> sourcesT;

        public Merge(
            IObservable<IAsyncObservable<TSource>> sources,
            int maxConcurrent = 0)
        {
            this.sources = sources.AsAsyncObservable();
            this.maxConcurrent = maxConcurrent;
        }

        public Merge(
            IEnumerable<IAsyncObservable<TSource>> sources,
            int maxConcurrent = 0)
        {
            this.sources = sources.ToAsyncObservable();
            this.maxConcurrent = maxConcurrent;
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
            if (this.maxConcurrent > 0)
            {
                var size = this.maxConcurrent;
                var functions = new AsyncList<Func<Task>>(new List<Func<Task>>());

                await this.sources.ForEachAsync((source, cancellationToken2) =>
                {
                    Func<Task> function = () => yield.ReturnAllAsync(source, cancellationToken2);
                    functions.Add(function);

                    return Task.CompletedTask;
                }, cancellationToken);

                for (var i = 0; i < Math.Ceiling(functions.Count / (double)size); i++)
                {
                    var partition = functions.Skip(size * i).Take(size);
                    var tasks = await partition.Select(function => function()).ToArrayAsync(cancellationToken);

                    await Task.WhenAll(tasks);
                }
            }
            else
            {
                var tasks = new List<Task>();

                await this.sources.ForEachAsync((source, cancellationToken2) =>
                {
                    tasks.Add(yield.ReturnAllAsync(source, cancellationToken2));

                    return Task.CompletedTask;
                }, cancellationToken);

                await Task.WhenAll(tasks);
            }
        }

        private async Task MergeImplTAsync(
            IAsyncYield<TSource> yield,
            CancellationToken cancellationToken)
        {
            var tasks = new List<Task>();

            await this.sourcesT.ForEachAsync((t, cancellationToken2) => 
            {
                var task = this.OnNextAsync(t, yield, cancellationToken2);
                tasks.Add(task);

                return Task.CompletedTask;
            }, cancellationToken);

            await Task.WhenAll(tasks);
        }

        private async Task OnNextAsync(
            Task<TSource> source,
            IAsyncYield<TSource> yield,
            CancellationToken cancellationToken)
        {
            var result = await source;

            await yield.ReturnAsync(result, cancellationToken);
        }
    }
}
