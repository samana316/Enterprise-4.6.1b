using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;

namespace Enterprise.Core.Reactive.Linq.Implementations
{
    internal sealed class SelectMany<TSource, TResult> : AsyncObservableBase<TResult>
    {
        private readonly IAsyncObservable<TSource> source;

        private readonly Func<TSource, IObservable<TResult>> selector;

        private readonly Func<TSource, IEnumerable<TResult>> selectorE;

        private readonly Func<TSource, int, IEnumerable<TResult>> selectorEI;

        private readonly Func<TSource, int, IObservable<TResult>> selectorI;

        private readonly Func<IObservable<TResult>> selectorOnCompleted;

        private readonly Func<Exception, IObservable<TResult>> selectorOnError;

        private readonly Func<TSource, CancellationToken, Task<TResult>> selectorT;

        private readonly Func<TSource, int, CancellationToken, Task<TResult>> selectorTI;

        private readonly IAsyncYieldBuilder<TResult> producer;

        private SelectMany(
            SelectMany<TSource, TResult> parent)
        {
            this.source = parent.source;
            this.selector = parent.selector;
            this.selectorE = parent.selectorE;
            this.selectorEI = parent.selectorEI;
            this.selectorI = parent.selectorI;
            this.selectorOnCompleted = parent.selectorOnCompleted;
            this.selectorOnError = parent.selectorOnError;
            this.selectorT = parent.selectorT;
            this.selectorTI = parent.selectorTI;
            this.producer = parent.producer;
        }

        public SelectMany(
            IAsyncObservable<TSource> source, 
            Func<TSource, IObservable<TResult>> selector)
        {
            this.source = source;
            this.selector = selector;
            this.producer = new SelectManyImpl(this);
        }

        public SelectMany(
            IAsyncObservable<TSource> source,
            Func<TSource, int, IObservable<TResult>> selector)
        {
            this.source = source;
            this.selectorI = selector;
            this.producer = new SelectManyImpl(this);
        }

        public SelectMany(
            IAsyncObservable<TSource> source,
            Func<TSource, IObservable<TResult>> onNext, 
            Func<Exception, IObservable<TResult>> onError,
            Func<IObservable<TResult>> onCompleted)
        {
            this.source = source;
            this.selector = onNext;
            this.selectorOnError = onError;
            this.selectorOnCompleted = onCompleted;
            this.producer = new SelectManyImpl(this);
        }

        public SelectMany(
            IAsyncObservable<TSource> source,
            Func<TSource, int, IObservable<TResult>> onNext,
            Func<Exception, IObservable<TResult>> onError,
            Func<IObservable<TResult>> onCompleted)
        {
            this.source = source;
            this.selectorI = onNext;
            this.selectorOnError = onError;
            this.selectorOnCompleted = onCompleted;
            this.producer = new SelectManyImpl(this);
        }

        public SelectMany(
            IAsyncObservable<TSource> source,
            Func<TSource, IEnumerable<TResult>> selector)
        {
            this.source = source;
            this.selectorE = selector;
            this.producer = new SelectManyImplE(this);
        }

        public SelectMany(
            IAsyncObservable<TSource> source,
            Func<TSource, int, IEnumerable<TResult>> selector)
        {
            this.source = source;
            this.selectorEI = selector;
            this.producer = new SelectManyImplE(this);
        }

        public SelectMany(
            IAsyncObservable<TSource> source,
            Func<TSource, CancellationToken, Task<TResult>> selector)
        {
            this.source = source;
            this.selectorT = selector;
            this.producer = new SelectManyImplT(this);
        }

        public SelectMany(
            IAsyncObservable<TSource> source,
            Func<TSource, int, CancellationToken, Task<TResult>> selector)
        {
            this.source = source;
            this.selectorTI = selector;
            this.producer = new SelectManyImplT(this);
        }

        public override AsyncIterator<TResult> Clone()
        {
            return new SelectMany<TSource, TResult>(this);
        }

        protected override Task ProduceAsync(
            IAsyncYield<TResult> yield, 
            CancellationToken cancellationToken)
        {
            return this.producer.RunAsync(yield, cancellationToken);
        }

        private sealed class SelectManyImpl : IAsyncYieldBuilder<TResult>
        {
            private readonly SelectMany<TSource, TResult> parent;

            public SelectManyImpl(
                SelectMany<TSource, TResult> parent)
            {
                this.parent = parent;
            }

            public async Task RunAsync(
                IAsyncYield<TResult> yield, 
                CancellationToken cancellationToken)
            {
                if (this.parent.selectorOnCompleted != null 
                    && this.parent.selectorOnError != null)
                {
                    try
                    {
                        await SelectorImplAsync(yield, cancellationToken);
                    }
                    catch (Exception exception)
                    {
                        await yield.ReturnAllAsync(this.parent.selectorOnError(exception), cancellationToken);
                    }
                    finally
                    {
                        await yield.ReturnAllAsync(this.parent.selectorOnCompleted(), cancellationToken);
                        yield.Break();
                    }
                }
                else
                {
                    await SelectorImplAsync(yield, cancellationToken);
                }
            }

            private async Task SelectorImplAsync(
                IAsyncYield<TResult> yield, 
                CancellationToken cancellationToken)
            {
                var tasks = new List<Task>();
                var index = 0;

                await this.parent.source.ForEachAsync((item, cancellationToken2) =>
                {
                    var results = this.parent.selectorI == null ?
                        this.parent.selector(item) : this.parent.selectorI(item, index++);

                    tasks.Add(yield.ReturnAllAsync(results, cancellationToken2));

                    return Task.CompletedTask;
                }, cancellationToken);

                await Task.WhenAll(tasks);
            }
        }

        private sealed class SelectManyImplE : IAsyncYieldBuilder<TResult>
        {
            private readonly SelectMany<TSource, TResult> parent;

            public SelectManyImplE(
                SelectMany<TSource, TResult> parent)
            {
                this.parent = parent;
            }

            public async Task RunAsync(
                IAsyncYield<TResult> yield, 
                CancellationToken cancellationToken)
            {
                var tasks = new List<Task>();
                var index = 0;

                await this.parent.source.ForEachAsync((item, cancellationToken2) =>
                {
                    var results = this.parent.selectorI == null ?
                        this.parent.selectorE(item) : this.parent.selectorEI(item, index++);

                    tasks.Add(yield.ReturnAllAsync(results, cancellationToken2));

                    return Task.CompletedTask;
                }, cancellationToken);

                await Task.WhenAll(tasks);
            }
        }

        private sealed class SelectManyImplT : IAsyncYieldBuilder<TResult>
        {
            private readonly SelectMany<TSource, TResult> parent;

            public SelectManyImplT(
                SelectMany<TSource, TResult> parent)
            {
                this.parent = parent;
            }

            public async Task RunAsync(
                IAsyncYield<TResult> yield,
                CancellationToken cancellationToken)
            {
                var tasks = new List<Task>();
                var index = 0;

                await this.parent.source.ForEachAsync((item, cancellationToken2) =>
                {
                    Func<Task> function = async () =>
                    {
                        var task = this.parent.selectorTI == null ?
                            this.parent.selectorT(item, cancellationToken2) : 
                            this.parent.selectorTI(item, index++, cancellationToken2);

                        await yield.ReturnAsync(await task, cancellationToken2);
                    };

                    tasks.Add(function());

                    return Task.CompletedTask;
                }, cancellationToken);

                await Task.WhenAll(tasks);
            }
        }
    }
}
