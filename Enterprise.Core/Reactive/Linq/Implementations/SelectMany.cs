using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Reactive.Linq.Implementations
{
    internal sealed class SelectMany<TSource, TResult> : AsyncObservableBase<TResult>
    {
        private readonly IAsyncObservable<TSource> source;

        private readonly Func<TSource, IObservable<TResult>> selector;

        private readonly Func<IAsyncYield<TResult>, CancellationToken, Task> implAsync;

        private SelectMany(
            SelectMany<TSource, TResult> instance)
        {
            Check.NotNull(instance);
            this.source = instance.source;
            this.selector = instance.selector;
            this.implAsync = instance.implAsync;
        }

        public SelectMany(
            IAsyncObservable<TSource> source, 
            Func<TSource, IObservable<TResult>> selector)
        {
            this.source = source;
            this.selector = selector;
        }

        public override AsyncIterator<TResult> Clone()
        {
            return new SelectMany<TSource, TResult>(this);
        }

        protected override Task ProduceAsync(
            IAsyncYield<TResult> yield, 
            CancellationToken cancellationToken)
        {
            return this.implAsync(yield, cancellationToken);
        }

        private async Task SelectManyImplAsync(
            IAsyncYield<TResult> yield,
            CancellationToken cancellationToken)
        {
            var tasks = new List<Task>();

            await this.source.ForEachAsync(async (item, cancellationToken2) =>
            {
                await Task.Yield();

                tasks.Add(yield.ReturnAllAsync(this.selector(item), cancellationToken2));
            }, cancellationToken);

            await Task.WhenAll(tasks);
        }
    }
}
