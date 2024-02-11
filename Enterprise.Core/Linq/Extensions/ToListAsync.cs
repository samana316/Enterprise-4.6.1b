using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        public static Task<List<TSource>> ToListAsync<TSource>(
            this IAsyncEnumerable<TSource> source)
        {
            return source.ToListAsync(CancellationToken.None);
        }

        public static async Task<List<TSource>> ToListAsync<TSource>(
            this IAsyncEnumerable<TSource> source,
            CancellationToken cancellationToken)
        {
            Check.NotNull(source, nameof(source));
            
            if (source is ICollection<TSource>)
            {
                return new List<TSource>(source);
            }

            var list = new List<TSource>();
            await source.ForEachAsync(list.Add, cancellationToken);

            return list;
        }
    }
}
