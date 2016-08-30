using System;
using System.Threading;
using System.Threading.Tasks;

namespace Enterprise.Core.Linq
{
    internal sealed class Buffer<TSource>
    {
        public TSource[] Array { get; private set; }

        public int Count { get; private set; }

        public static async Task<Buffer<TSource>> CreateAsync(
            IAsyncEnumerable<TSource> source,
            CancellationToken cancellationToken)
        {
            var ret = new TSource[16];
            var tmpCount = 0;

            await source.ForEachAsync((item) =>
            {
                if (tmpCount == ret.Length)
                {
                    System.Array.Resize(ref ret, ret.Length * 2);
                }
                ret[tmpCount++] = item;
            }, cancellationToken);

            return new Buffer<TSource>
            {
                Array = ret,
                Count = tmpCount
            };
        }
    }
}
