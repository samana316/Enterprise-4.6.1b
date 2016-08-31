using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        public static Task<TSource[]> ToArrayAsync<TSource>(
            this IAsyncEnumerable<TSource> source)
        {
            return source.ToArrayAsync(CancellationToken.None);
        }

        public static async Task<TSource[]> ToArrayAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            CancellationToken cancellationToken)
        {
            Check.NotNull(source, nameof(source));

            var collection = source as ICollection<TSource>;
            if (collection != null)
            {
                var array = new TSource[collection.Count];
                collection.CopyTo(array, 0);

                return array;
            }

            var buffer = await source.ToBufferAsync(cancellationToken);
            return GetResizedArray(buffer);
        }

        private static TSource[] GetResizedArray<TSource>(
            Buffer<TSource> buffer)
        {
            var count = buffer.Count;
            var ret = buffer.Array;

            if (count != ret.Length)
            {
                Array.Resize(ref ret, count);
            }

            return ret;
        }
    }
}
