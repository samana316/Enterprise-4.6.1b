using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Linq;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Reactive.Linq
{
    using Implementations;

    public static partial class AsyncObservable
    {
        public static IAsyncObservable<T> Create<T>(
            Func<IAsyncYield<T>, CancellationToken, Task> producer)
        {
            Check.NotNull(producer, nameof(producer));

            return new Anonymous<T>(producer);
        }
    }
}
