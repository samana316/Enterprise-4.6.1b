using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;

namespace Enterprise.Core.Reactive.Linq.Implementations
{
    internal sealed class Catch<TSource, TException> : AsyncObservableBase<TSource>
        where TException : Exception
    {
        private readonly IAsyncObservable<TSource> source;

        private readonly Func<TException, IObservable<TSource>> handler;

        public Catch(
            IAsyncObservable<TSource> source, 
            Func<TException, IObservable<TSource>> handler)
        {
            this.source = source;
            this.handler = handler;
        }

        public override AsyncIterator<TSource> Clone()
        {
            return new Catch<TSource, TException>(this.source, this.handler);
        }

        protected override async Task ProduceAsync(
            IAsyncYield<TSource> yield, 
            CancellationToken cancellationToken)
        {
            try
            {
                await yield.ReturnAllAsync(this.source, cancellationToken);
            }
            catch (TException exception)
            {
                var result = this.handler(exception);
                await yield.ReturnAllAsync(result, cancellationToken);
            }
        }
    }

    internal sealed class Catch<TSource> : AsyncObservableBase<TSource>
    {
        private readonly IAsyncEnumerable<IAsyncObservable<TSource>> sources;

        public Catch(
            IEnumerable<IAsyncObservable<TSource>> sources)
        {
            this.sources = sources.AsAsyncEnumerable();
        }

        public override AsyncIterator<TSource> Clone()
        {
            return new Catch<TSource>(this.sources);
        }

        protected override Task ProduceAsync(
            IAsyncYield<TSource> yield, 
            CancellationToken cancellationToken)
        {
            var terminate = false;

            return this.sources.ForEachAsync(async (source, cancellationToken2) => 
            {
                if (terminate)
                {
                    yield.Break();
                }

                try
                {
                    await yield.ReturnAllAsync(source, cancellationToken2);
                    terminate = true;
                }
                catch
                {
                    terminate = false;
                }
            }, cancellationToken);
        }
    }
}
