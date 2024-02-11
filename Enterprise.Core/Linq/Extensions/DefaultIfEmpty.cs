using Enterprise.Core.Utilities;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        public static IAsyncEnumerable<TSource> DefaultIfEmpty<TSource>(
            this IAsyncEnumerable<TSource> source)
        {
            return source.DefaultIfEmpty(default(TSource));
        }

        public static IAsyncEnumerable<TSource> DefaultIfEmpty<TSource>(
            this IAsyncEnumerable<TSource> source,
            TSource defaultValue)
        {
            Check.NotNull(source, nameof(source));

            return new DefaultIfEmpty<TSource>(source, defaultValue);
        }
    }
}
