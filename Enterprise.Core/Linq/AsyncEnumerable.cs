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
    }
}
