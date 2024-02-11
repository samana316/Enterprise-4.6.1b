using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Linq
{
    public static partial class AsyncEnumerable
    {
        public static IAsyncEnumerable<T> Create<T>(
            Func<IAsyncYield<T>, CancellationToken, Task> producer)
        {
            Check.NotNull(producer, nameof(producer));

            return new Anonymous<T>(producer);
        }

        internal static Task<Buffer<TSource>> ToBufferAsync<TSource>(
            this IAsyncEnumerable<TSource> source, 
            CancellationToken cancellationToken)
        {
            Check.NotNull(source, nameof(source));

            return Buffer<TSource>.CreateAsync(source, cancellationToken);
        }

        private static class IdentityFunction<TElement>
        {
            public static Func<TElement, TElement> Instance
            {
                get { return IdentityFunctionImpl; }
            }

            private static TElement IdentityFunctionImpl(
                TElement x)
            {
                return x;
            }
        }
    }
}
