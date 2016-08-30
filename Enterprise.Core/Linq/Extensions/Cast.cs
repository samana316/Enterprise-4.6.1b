using Enterprise.Core.Utilities;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        public static IAsyncEnumerable<TResult> Cast<TResult>(
            this IAsyncEnumerable source)
        {
            Check.NotNull(source, nameof(source));

            var existing = source as IAsyncEnumerable<TResult>;
            if (existing != null)
            {
                return existing;
            }

            return new Cast<TResult>(source);
        }
    }
}
