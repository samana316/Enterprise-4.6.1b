using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;

namespace Enterprise.Core.Reactive.Linq.Implementations
{
    internal sealed class Repeat<TResult> : AsyncObservableBase<TResult>
    {
        private readonly TResult element;

        private readonly IAsyncObservable<TResult> source;

        private readonly int? count;

        public Repeat(
            TResult element,
            int? count)
        {
            this.element = element;
            this.count = count;
        }

        public Repeat(
            IAsyncObservable<TResult> source, 
            int? count)
        {
            this.source = source;
            this.count = count;
        }

        public override AsyncIterator<TResult> Clone()
        {
            if (this.source != null)
            {
                return new Repeat<TResult>(this.source, this.count);
            }

            return new Repeat<TResult>(this.element, this.count);
        }

        protected override async Task ProduceAsync(
            IAsyncYield<TResult> yield,
            CancellationToken cancellationToken)
        {
            if (this.count.HasValue)
            {
                for (int i = 0; i < this.count.Value; i++)
                {
                    await this.OnNextAsync(yield, cancellationToken);
                }
            }
            else
            {
                while (true)
                {
                    await this.OnNextAsync(yield, cancellationToken);
                }
            }
        }

        private Task OnNextAsync(
            IAsyncYield<TResult> yield, 
            CancellationToken cancellationToken)
        {
            if (this.source != null)
            {
                return yield.ReturnAllAsync(this.source, cancellationToken);
            }

            return yield.ReturnAsync(this.element, cancellationToken);
        }
    }
}
