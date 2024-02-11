using System.Threading;
using System.Threading.Tasks;

namespace Enterprise.Core.Linq
{
    internal sealed class Repeat<TResult> : AsyncEnumerableBase<TResult>
    {
        private readonly TResult element;

        private readonly int count;

        public Repeat(
            TResult element, 
            int count)
        {
            this.element = element;
            this.count = count;
        }

        public override AsyncIterator<TResult> Clone()
        {
            return new Repeat<TResult>(this.element, this.count);
        }

        protected override async Task EnumerateAsync(
            IAsyncYield<TResult> yield, 
            CancellationToken cancellationToken)
        {
            for (int i = 0; i < this.count; i++)
            {
                await yield.ReturnAsync(this.element, cancellationToken);
            }
        }
    }
}
