using Enterprise.Core.Reactive.Linq.Implementations;
using Enterprise.Core.Resources;

namespace Enterprise.Core.Reactive.Linq
{
    partial class AsyncObservable
    {
        public static IAsyncObservable<TResult> Repeat<TResult>(
            TResult element)
        {
            return new Repeat<TResult>(element, null);
        }

        public static IAsyncObservable<TResult> Repeat<TResult>(
            TResult element,
            int count)
        {
            if (count < 0)
            {
                throw Error.ArgumentOutOfRange(nameof(count));
            }

            if (count == 0)
            {
                return Empty<TResult>();
            }

            if (count == 1)
            {
                return Return(element);
            }

            return new Repeat<TResult>(element, count);
        }
    }
}
