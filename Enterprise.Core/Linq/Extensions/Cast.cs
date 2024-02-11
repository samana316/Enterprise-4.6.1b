using Enterprise.Core.Utilities;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        public static IAsyncEnumerable<TResult> Cast<TResult>(
            this IAsyncEnumerable source)
        {
            Check.NotNull(source, nameof(source));

            IAsyncEnumerable<TResult> asyncEnumerable;
            if (source.TryAsAsyncEnumerable(out asyncEnumerable))
            {
                return asyncEnumerable;
            }

            if (source.TryCast(out asyncEnumerable))
            {
                return asyncEnumerable;
            }

            return new Cast<TResult>(source);
        }
    }
}
