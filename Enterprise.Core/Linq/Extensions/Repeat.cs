using Enterprise.Core.Resources;

namespace Enterprise.Core.Linq
{
    partial class AsyncEnumerable
    {
        public static IAsyncEnumerable<TResult> Repeat<TResult>(
            TResult element,
            int count)
        {
            if (count < 0)
            {
                throw Error.ArgumentOutOfRange(nameof(count));
            }

            return new Repeat<TResult>(element, count);
        }
    }
}