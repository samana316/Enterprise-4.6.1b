using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        public static async Task ForEachAsync(
            this IAsyncEnumerable source,
            Func<object, CancellationToken, Task> function,
            CancellationToken cancellationToken)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(function, nameof(function));

            using (var enumerator = source.GetAsyncEnumerator())
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (await enumerator.MoveNextAsync(cancellationToken).WithCurrentCulture())
                {
                    Task<bool> moveNextTask;
                    do
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        var current = enumerator.Current;
                        moveNextTask = enumerator.MoveNextAsync(cancellationToken);
                        await function(current, cancellationToken);
                    }
                    while (await moveNextTask.WithCurrentCulture());
                }
            }
        }

        public static async Task ForEachAsync<T>(
            this IAsyncEnumerable<T> source,
            Func<T, CancellationToken, Task> function,
            CancellationToken cancellationToken)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(function, nameof(function));

            using (var enumerator = source.GetAsyncEnumerator())
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (await enumerator.MoveNextAsync(cancellationToken).WithCurrentCulture())
                {
                    Task<bool> moveNextTask;
                    do
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        var current = enumerator.Current;
                        moveNextTask = enumerator.MoveNextAsync(cancellationToken);
                        await function(current, cancellationToken);
                    }
                    while (await moveNextTask.WithCurrentCulture());
                }
            }
        }
    }
}
