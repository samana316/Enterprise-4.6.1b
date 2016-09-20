using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Common.Runtime.CompilerServices;
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

            return new ForEachAsync<TSource>(source, onNextAsync, cancellationToken).ToTask();
        }
    }
}
