using Enterprise.Core.Reactive.Linq.Implementations;
using Enterprise.Core.Resources;

namespace Enterprise.Core.Reactive.Linq
{
    partial class AsyncObservable
    {
        public static IAsyncObservable<int> Range(
            int start,
            int count)
        {
            var num = (long)start + count - 1L;

            if (count < 0 || num > int.MaxValue)
            {
                throw Error.ArgumentOutOfRange(nameof(count));
            }

            if (count == 0)
            {
                return Empty<int>();
            }

            if (count == 1)
            {
                return Return(start);
            }

            return new Range(start, count);
        }
    }
}
