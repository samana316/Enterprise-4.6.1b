using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Linq
{
    public static class AsyncEnumerator
    {
        public static IAsyncEnumerator<T> Create<T>(
            Func<IAsyncYield<T>, CancellationToken, Task> producer)
        {
            Check.NotNull(producer, nameof(producer));

            return new Anonymous<T>(producer).GetAsyncEnumerator();
        }

        public static Task<bool> MoveNextAsync(
           this IAsyncEnumerator enumerator)
        {
            Check.NotNull(enumerator, nameof(enumerator));

            return enumerator.MoveNextAsync(CancellationToken.None);
        }

        public static Task<bool> MoveNextAsync(
            this IEnumerator enumerator)
        {
            Check.NotNull(enumerator, nameof(enumerator));

            return enumerator.MoveNextAsync(CancellationToken.None);
        }

        public static Task<bool> MoveNextAsync(
            this IEnumerator enumerator,
            CancellationToken cancellationToken)
        {
            Check.NotNull(enumerator, nameof(enumerator));

            var asyncEnumerator = enumerator as IAsyncEnumerator;
            if (asyncEnumerator != null)
            {
                return asyncEnumerator.MoveNextAsync(cancellationToken);
            }

            Func<bool> function = enumerator.MoveNext;
            var task = Task.Run(function, cancellationToken);

            while (!cancellationToken.IsCancellationRequested)
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    break;
                }

                cancellationToken.ThrowIfCancellationRequested();
            }

            return task;
        }
    }
}
