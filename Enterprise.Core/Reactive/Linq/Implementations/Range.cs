using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;

namespace Enterprise.Core.Reactive.Linq.Implementations
{
    internal sealed class Range : AsyncObservableBase<int>
    {
        private readonly int start;

        private readonly int count;

        public Range(
            int start,
            int count)
        {
            this.start = start;
            this.count = count;
        }

        public override AsyncIterator<int> Clone()
        {
            return new Range(this.start, this.count);
        }

        protected override async Task ProduceAsync(
            IAsyncYield<int> yield,
            CancellationToken cancellationToken)
        {
            for (var i = 0; i < this.count; i++)
            {
                await yield.ReturnAsync(start + i, cancellationToken);
            }
        }
    }
}
