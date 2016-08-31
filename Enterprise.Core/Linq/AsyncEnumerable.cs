using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Linq
{
    public static partial class AsyncEnumerable
    {
        public static IAsyncEnumerable<T> Create<T>(
            Func<IAsyncYield<T>, CancellationToken, Task> yieldBuilder)
        {
            Check.NotNull(yieldBuilder, nameof(yieldBuilder));

            return new Anonymous<T>(yieldBuilder);
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
