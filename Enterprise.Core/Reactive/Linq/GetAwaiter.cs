using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Resources;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Reactive.Linq
{
    partial class AsyncObservable
    {
        public static TaskAwaiter<TSource> GetAwaiter<TSource>(
            this IAsyncObservable<TSource> source)
        {
            Check.NotNull(source, nameof(source));

            var task = source.GetAwaiterImpl();

            return task.GetAwaiter();
        }

        private static async Task<TSource> GetAwaiterImpl<TSource>(
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