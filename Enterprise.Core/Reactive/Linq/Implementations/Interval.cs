using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;

namespace Enterprise.Core.Reactive.Linq.Implementations
{
    internal sealed class Interval : AsyncObservableBase<long>
    {
        private readonly TimeSpan period;

        public Interval(
            TimeSpan period)
        {
            this.period = period;
        }

        public override AsyncIterator<long> Clone()
        {
            return new Interval(this.period);
        }

        protected override async Task ProduceAsync(
            IAsyncYield<long> yield, 
            CancellationToken cancellationToken)
        {
            var i = 0L;

            while (true)
            {
                await Task.Delay(this.period, cancellationToken);
                await yield.ReturnAsync(i, cancellationToken);
                i++;
            };
        }
    }
}
