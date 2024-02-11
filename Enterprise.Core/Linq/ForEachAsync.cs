using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        public static Task ForEachAsync(
            this IAsyncEnumerable source,
            Action<object> action)
        {
            return source.ForEachAsync(action, CancellationToken.None);
        }

        public static Task ForEachAsync<T>(
            this IAsyncEnumerable<T> source,
            Action<T> action)
        {
            return source.ForEachAsync(action, CancellationToken.None);
        }

        // <summary>
        // Asynchronously executes the provided action on each element of the <see cref="IAsyncEnumerable" />.
        // </summary>
        // <param name="action"> The action to be executed. </param>
        // <param name="cancellationToken"> The token to monitor for cancellation requests. </param>
        // <returns> A Task representing the asynchronous operation. </returns>
        public static async Task ForEachAsync(
            this IAsyncEnumerable source, Action<object> action, CancellationToken cancellationToken)
        {
            Check.NotNull(source, "source");
            Check.NotNull(action, "action");

            cancellationToken.ThrowIfCancellationRequested();

            using (var enumerator = source.GetAsyncEnumerator())
            {
                if (await enumerator.MoveNextAsync(cancellationToken).WithCurrentCulture())
                {
                    Task<bool> moveNextTask;
                    do
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        var current = enumerator.Current;
                        moveNextTask = enumerator.MoveNextAsync(cancellationToken);
                        action(current);
                    }
                    while (await moveNextTask.WithCurrentCulture());
                }
            }
        }

        // <summary>
        // Asynchronously executes the provided action on each element of the <see cref="IAsyncEnumerable{T}" />.
        // </summary>
        // <param name="action"> The action to be executed. </param>
        // <param name="cancellationToken"> The token to monitor for cancellation requests. </param>
        // <returns> A Task representing the asynchronous operation. </returns>
        public static Task ForEachAsync<T>(
            this IAsyncEnumerable<T> source, Action<T> action, CancellationToken cancellationToken)
        {
            Check.NotNull(source, "source");
            Check.NotNull(action, "action");

            return ForEachAsync(source.GetAsyncEnumerator(), action, cancellationToken);
        }

        private static async Task ForEachAsync<T>(
            IAsyncEnumerator<T> enumerator, Action<T> action, CancellationToken cancellationToken)
        {
            using (enumerator)
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
                        action(current);
                    }
                    while (await moveNextTask.WithCurrentCulture());
                }
            }
        }

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

                while (await enumerator.MoveNextAsync(cancellationToken).WithCurrentCulture())
                {
                    await function(enumerator.Current, cancellationToken);
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

                while (await enumerator.MoveNextAsync(cancellationToken).WithCurrentCulture())
                {
                    await function(enumerator.Current, cancellationToken);
                }
            }
        }
    }
}
