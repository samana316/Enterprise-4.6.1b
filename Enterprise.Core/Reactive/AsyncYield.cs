using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Reactive.Linq
{
    public static class AsyncYield
    {
        public static Task ReturnAllAsync<TResult>(
            this IAsyncYield<TResult> yield,
            IObservable<TResult> source)
        {
            return yield.ReturnAllAsync(source, CancellationToken.None);
        }

        public static Task ReturnAllAsync<TResult>(
            this IAsyncYield<TResult> yield,
            IObservable<TResult> source,
            CancellationToken cancellationToken)
        {
            return yield.ReturnAllAsync(source.AsAsyncObservable(), cancellationToken);
        }

        public static Task ReturnAllAsync<TResult>(
            this IAsyncYield<TResult> yield,
            IAsyncObservable<TResult> source)
        {
            return yield.ReturnAllAsync(source, CancellationToken.None);
        }

        public static Task ReturnAllAsync<TResult>(
            this IAsyncYield<TResult> yield,
            IAsyncObservable<TResult> source,
            CancellationToken cancellationToken)
        {
            Check.NotNull(yield, nameof(yield));
            Check.NotNull(source, nameof(source));

            return source.ForEachAsync((item, cancellationToken2) =>
            {
                return yield.ReturnAsync(item, cancellationToken2);
            }, cancellationToken);
        }
    }
}
