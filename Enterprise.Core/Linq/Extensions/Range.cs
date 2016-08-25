using Enterprise.Core.Resources;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        public static IAsyncEnumerable<int> Range(
            int start,
            int count)
        {
            var num = (long)start + count - 1L;

            if (count < 0 || num > 2147483647L)
            {
                throw Error.ArgumentOutOfRange("count");
            }

            return new Range(start, count);
        }
    }
}