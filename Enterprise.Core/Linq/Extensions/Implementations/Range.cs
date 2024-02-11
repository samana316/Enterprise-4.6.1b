using System.Threading;
using System.Threading.Tasks;

namespace Enterprise.Core.Linq
{
    internal sealed class Range : AsyncEnumerableBase<int>
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

        protected override async Task EnumerateAsync(
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
