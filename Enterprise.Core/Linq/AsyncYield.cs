using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Linq
{
    public static class AsyncYield
    {
        public static Task ReturnAllAsync<TResult>(
            this IAsyncYield<TResult> yield,
            IEnumerable<TResult> source)
        {
            return yield.ReturnAllAsync(source, CancellationToken.None);
        }

        public static Task ReturnAllAsync<TResult>(
            this IAsyncYield<TResult> yield,
            IEnumerable<TResult> source,
            CancellationToken cancellationToken)
        {
            Check.NotNull(yield, nameof(yield));
            Check.NotNull(source, nameof(source));

            return source.AsAsyncEnumerable().ForEachAsync((item, cancellationToken2) => 
            {
                return yield.ReturnAsync(item, cancellationToken2);
            }, cancellationToken);
        }

        public static Task ReturnAsync<TResult>(
            this IAsyncYield<TResult> yield,
            TResult value)
        {
            Check.NotNull(yield, nameof(yield));

            return yield.ReturnAsync(value, CancellationToken.None);
        }
    }
}
