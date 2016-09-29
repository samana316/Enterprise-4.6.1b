using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;

namespace Enterprise.Core.Reactive.Linq.Implementations
{
    internal sealed class Switch<TSource> : AsyncObservableBase<TSource>
    {
        private readonly IAsyncObservable<IAsyncObservable<TSource>> sources;

        private readonly IAsyncObservable<Task<TSource>> sourcesT;

        public Switch(
            IAsyncObservable<Task<TSource>> sources)
        {
            this.sourcesT = sources;
        }

        public Switch(
            IObservable<IAsyncObservable<TSource>> sources)
        {
            this.sources = sources.AsAsyncObservable();
        }

        public override AsyncIterator<TSource> Clone()
        {
            if (this.sources == null)
            {
                return new Switch<TSource>(this.sourcesT);
            }

            return new Switch<TSource>(this.sources);
        }

        protected override Task ProduceAsync(
            IAsyncYield<TSource> yield, 
            CancellationToken cancellationToken)
        {
            IAsyncYieldBuilder<TSource> producer;

            if (this.sources == null)
            {
                producer = new SwitchImplT(this);
            }
            else
            {
                producer = new SwitchImpl(this);
            }

            return producer.RunAsync(yield, cancellationToken);
        }
        
        private sealed class SwitchImpl : IAsyncYieldBuilder<TSource>
        {
            private readonly object sink = new object();

            private readonly ICollection<IAsyncObservable<TSource>> history = new List<IAsyncObservable<TSource>>();

            private readonly Switch<TSource> parent;

            private IAsyncObservable<TSource> current;

            public SwitchImpl(
                Switch<TSource> parent)
            {
                this.parent = parent;
            }

            public async Task RunAsync(
                IAsyncYield<TSource> yield, 
                CancellationToken cancellationToken)
            {
                var tasks = new List<Task>();

                await this.parent.sources.ForEachAsync((source, cancellationToken2) =>
                {
                    var task = this.OnNextAsync(source, yield, cancellationToken2);
                    tasks.Add(task);

                    return Task.CompletedTask;
                }, cancellationToken);

                await Task.WhenAll(tasks);
            }

            private Task OnNextAsync(
                IAsyncObservable<TSource> source,
                IAsyncYield<TSource> yield,
                CancellationToken cancellationToken)
            {
                return source.ForEachAsync((value, cancellationToken2) => 
                {
                    lock (sink)
                    {
                        if (this.current != source)
                        {
                            if (this.history.Contains(source))
                            {
                                this.history.Remove(source);
                                yield.Break();
                            }
                            else
                            {
                                this.history.Add(source);
                                this.current = source;
                            }
                        }
                    }

                    return yield.ReturnAsync(value, cancellationToken2);
                }, cancellationToken);
            }
        }

        private sealed class SwitchImplT : IAsyncYieldBuilder<TSource>
        {
            private readonly object sink = new object();

            private readonly ICollection<Task<TSource>> history = new List<Task<TSource>>();

            private readonly Switch<TSource> parent;

            private Task<TSource> current;

            public SwitchImplT(
                Switch<TSource> parent)
            {
                this.parent = parent;
            }

            public async Task RunAsync(
                IAsyncYield<TSource> yield,
                CancellationToken cancellationToken)
            {
                var tasks = new List<Task>();

                await this.parent.sourcesT.ForEachAsync((source, cancellationToken2) =>
                {
                    var task = this.OnNextAsync(source, yield, cancellationToken2);
                    tasks.Add(task);

                    return Task.CompletedTask;
                }, cancellationToken);

                await Task.WhenAll(tasks);
            }

            private Task OnNextAsync(
                Task<TSource> source,
                IAsyncYield<TSource> yield,
                CancellationToken cancellationToken)
            {
                var checkTask = Task.Run(() => 
                {
                    lock (sink)
                    {
                        if (this.current != source)
                        {
                            if (this.history.Contains(source))
                            {
                                this.history.Remove(source);
                                yield.Break();
                            }
                            else
                            {
                                this.history.Add(source);
                                this.current = source;
                            }
                        }
                    }
                });

                var yieldTask = Task.Run(async () => 
                {
                    await yield.ReturnAsync(await source, cancellationToken);
                });

                return Task.WhenAny(checkTask, yieldTask);
            }
        }
    }
}
