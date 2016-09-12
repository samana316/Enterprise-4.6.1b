using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;

namespace Enterprise.Core.Reactive.Linq.Implementations
{
    internal sealed class Repeat<TResult> : AsyncObservableBase<TResult>
    {
        private readonly TResult element;

        private readonly int? count;

        public Repeat(
            TResult element,
            int? count)
        {
            this.element = element;
            this.count = count;
        }

        public override AsyncIterator<TResult> Clone()
        {
            return new Repeat<TResult>(this.element, this.count);
        }

        protected override async Task ProduceAsync(
            IAsyncYield<TResult> yield,
            CancellationToken cancellationToken)
        {
            if (!this.count.HasValue)
            {
                while (true)
                {
                    await yield.ReturnAsync(this.element, cancellationToken);
                }
            }
            else
            {
                for (int i = 0; i < this.count.Value; i++)
                {
                    await yield.ReturnAsync(this.element, cancellationToken);
                }
            }
        }
    }
}
