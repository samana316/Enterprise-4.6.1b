using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;

namespace Enterprise.Core.Reactive.Linq.Implementations
{
    internal sealed class Delay<TSource> : AsyncObservableBase<TSource>
    {
        private readonly IAsyncObservable<TSource> source;

        private readonly TimeSpan? dueTime;

        private readonly DateTimeOffset? dueTimeO;

        public Delay(
            IAsyncObservable<TSource> source,
            TimeSpan dueTime)
        {
            this.source = source;
            this.dueTime = dueTime;
        }

        public Delay(
            IAsyncObservable<TSource> source,
            DateTimeOffset dueTime)
        {
            this.source = source;
            this.dueTimeO = dueTime;
        }

        public override AsyncIterator<TSource> Clone()
        {
            if (this.dueTimeO.HasValue)
            {
                return new Delay<TSource>(this.source, this.dueTimeO.Value);
            }

            return new Delay<TSource>(this.source, this.dueTime.Value);
        }

        protected override async Task ProduceAsync(
            IAsyncYield<TSource> yield,
            CancellationToken cancellationToken)
        {
            var delay = this.dueTime.HasValue ? this.dueTime.Value : this.dueTimeO.Value - DateTimeOffset.Now;

            await Task.Delay(delay, cancellationToken);
            await yield.ReturnAllAsync(this.source, cancellationToken);
        }
    }
}
