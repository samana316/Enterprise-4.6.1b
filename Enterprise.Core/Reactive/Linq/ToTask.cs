using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Resources;

namespace Enterprise.Core.Reactive.Linq
{
    partial class AsyncObservable
    {
        public static async Task<TSource> ToTask<TSource>(
            this IAsyncObservable<TSource> source)
        {
            var flag = false;
            var final = default(TSource);

            await source.ForEachAsync(async (item, cancellationToken) =>
            {
                await Task.Yield();

                final = item;
                flag = true;
            }, CancellationToken.None);

            if (flag)
            {
                return final;
            }

            throw Error.EmptySequence();
        }
    }
}