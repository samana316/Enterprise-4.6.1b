using System.Threading;
using System.Threading.Tasks;

namespace Enterprise.Core.Linq
{
    internal sealed class Cast<TResult> : AsyncEnumerableBase<TResult>
    {
        private readonly IAsyncEnumerable source;

        public Cast(
            IAsyncEnumerable source)
        {
            this.source = source;
        }

        public override AsyncIterator<TResult> Clone()
        {
            return new Cast<TResult>(this.source);
        }

        protected override Task EnumerateAsync(
            IAsyncYield<TResult> yield, 
            CancellationToken cancellationToken)
        {
            return this.source.ForEachAsync((item, cancellationToken2) => 
            {
                return yield.ReturnAsync((TResult)item, cancellationToken2);
            }, cancellationToken);
        }
    }
}
