using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Reactive.Linq.Implementations;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Reactive.Linq
{
    partial class AsyncObservable
    {
        public static Task ForEachAsync<TSource>(
            this IAsyncObservable<TSource> source,
            Func<TSource, CancellationToken, Task> onNextAsync,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(source, nameof(source));
            Check.NotNull(onNextAsync, nameof(onNextAsync));

            return source.ForEachImplAsync(onNextAsync, cancellationToken);
        }

        private static async Task ForEachImplAsync<TSource>(
            this IAsyncObservable<TSource> source,
            Func<TSource, CancellationToken, Task> onNextAsync,
            CancellationToken cancellationToken)
        {
            await new ForEachAsync<TSource>(source, onNextAsync, cancellationToken);
        }
    }
}
