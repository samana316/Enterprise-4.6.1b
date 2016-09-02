using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Resources;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        public static Task<TSource> ElementAtAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            int index)
        {
            Check.NotNull(source, "source");

            return source.ElementAtAsync(index, CancellationToken.None);
        }

        public static async Task<TSource> ElementAtAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            int index,
            CancellationToken cancellationToken)
        {
            Check.NotNull(source, "source");
            cancellationToken.ThrowIfCancellationRequested();

            var list = source as IReadOnlyList<TSource>;
            if (list != null)
            {
                return list[index];
            }

            if (index < 0)
            {
                throw Error.ArgumentOutOfRange("index");
            }

            using (var enumerator = source.GetAsyncEnumerator())
            {
                while (await enumerator.MoveNextAsync(cancellationToken).WithCurrentCulture())
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (index == 0)
                    {
                        return enumerator.Current;
                    }

                    index--;
                }
            }

            throw Error.ArgumentOutOfRange("index");
        }

        public static Task<TSource> ElementAtOrDefaultAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            int index)
        {
            Check.NotNull(source, "source");

            return source.ElementAtOrDefaultAsync(index, CancellationToken.None);
        }

        public static async Task<TSource> ElementAtOrDefaultAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            int index,
            CancellationToken cancellationToken)
        {
            Check.NotNull(source, "source");
            cancellationToken.ThrowIfCancellationRequested();

            var list = source as IReadOnlyList<TSource>;
            if (list != null)
            {
                return list[index];
            }

            if (index >= 0)
            {
                using (var enumerator = source.GetAsyncEnumerator())
                {
                    while (await enumerator.MoveNextAsync(cancellationToken).WithCurrentCulture())
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        if (index == 0)
                        {
                            return enumerator.Current;
                        }

                        index--;
                    }
                }
            }

            return default(TSource);
        }

        private static class AlwaysTrue<TElement>
        {
            public static Func<TElement, bool> Instance
            {
                get { return AlwaysTrueImpl; }
            }

            private static bool AlwaysTrueImpl(
                TElement element)
            {
                return true;
            }
        }
    }
}
