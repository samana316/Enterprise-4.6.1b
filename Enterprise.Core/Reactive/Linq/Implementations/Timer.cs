using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;

namespace Enterprise.Core.Reactive.Linq.Implementations
{
    internal sealed class Timer : AsyncObservableBase<long>
    {
        private readonly TimeSpan? dueTime;

        private readonly TimeSpan? period;

        private readonly DateTimeOffset? dueTimeO;

        public Timer(
            TimeSpan dueTime, 
            TimeSpan? period = null)
        {
            this.dueTime = dueTime;
            this.period = period;
        }

        public Timer(
            DateTimeOffset dueTime, 
            TimeSpan? period = null)
        {
            this.dueTimeO = dueTime;
            this.period = period;
        }

        public override AsyncIterator<long> Clone()
        {
            throw new NotImplementedException();
        }

        protected override async Task ProduceAsync(
            IAsyncYield<long> yield, 
            CancellationToken cancellationToken)
        {
            var value = 0L;

            do
            {
                var delay = this.TimeSelector(value);
                await Task.Delay(delay, cancellationToken);
                await yield.ReturnAsync(value, cancellationToken);
                value++;
            }
            while (this.Condition(value));
        }

        private bool Condition(
            long value)
        {
            return this.period.HasValue ? true : value < 1;
        }

        private TimeSpan TimeSelector(
            long value)
        {
            if (value > 0)
            {
                return this.period.GetValueOrDefault();
            }

            return this.dueTime.HasValue ? this.dueTime.Value : this.dueTimeO.Value - DateTimeOffset.Now;
        }
    }
}
