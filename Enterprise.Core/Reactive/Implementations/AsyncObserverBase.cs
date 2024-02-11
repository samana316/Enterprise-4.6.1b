using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Common;
using Enterprise.Core.Common.Runtime.ExceptionServices;

namespace Enterprise.Core.Reactive
{
    internal abstract class AsyncObserverBase<T> : DisposableBase, IAsyncObserver<T>
    {
        public virtual void OnCompleted()
        {
        }

        public virtual void OnError(
            Exception error)
        {
            error.Rethrow();
        }

        public virtual void OnNext(
            T value)
        {
            throw new NotSupportedException();
        }

        public abstract Task OnNextAsync(
            T value,
            CancellationToken cancellationToken);
    }
}
