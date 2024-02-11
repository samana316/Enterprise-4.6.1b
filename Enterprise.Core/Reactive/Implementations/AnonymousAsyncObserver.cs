using System;
using System.Threading;
using System.Threading.Tasks;

namespace Enterprise.Core.Reactive
{
    internal sealed class AnonymousAsyncObserver<T> : AsyncObserverBase<T>
    {
        private readonly Func<T, CancellationToken, Task> onNextAsync;

        private readonly Action<Exception> onError;

        private readonly Action onCompleted;

        public AnonymousAsyncObserver(
            Func<T, CancellationToken, Task> onNextAsync, 
            Action<Exception> onError = null, 
            Action onCompleted = null)
        {
            this.onNextAsync = onNextAsync;
            this.onError = onError;
            this.onCompleted = onCompleted;
        }

        public override void OnCompleted()
        {
            if (this.onCompleted != null)
            {
                this.onCompleted();
            }
        }

        public override void OnError(
            Exception error)
        {
            if (this.onError != null)
            {
                this.onError(error);
            }
            else
            {
                base.OnError(error);
            }
        }

        public override Task OnNextAsync(
            T value, 
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.onNextAsync(value, cancellationToken);
        }
    }
}
