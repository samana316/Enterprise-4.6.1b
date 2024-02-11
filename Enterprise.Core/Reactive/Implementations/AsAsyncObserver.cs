using System;
using System.Threading;
using System.Threading.Tasks;

namespace Enterprise.Core.Reactive
{
    internal sealed class AsAsyncObserver<T> : AsyncObserverBase<T>
    {
        private readonly IObserver<T> observer;

        public AsAsyncObserver(
            IObserver<T> observer)
        {
            this.observer = observer;
        }

        public override void OnCompleted()
        {
            this.observer.OnCompleted();
            base.OnCompleted();
        }

        public override void OnError(
            Exception error)
        {
            this.observer.OnError(error);
            base.OnError(error);
        }

        public override Task OnNextAsync(
            T value, 
            CancellationToken cancellationToken)
        {
            var builder = new OnNextBuilder(this.observer, value);
            var task = Task.Run(new Action(builder.OnNext), cancellationToken);

            while (!cancellationToken.IsCancellationRequested)
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    break;
                }

                cancellationToken.ThrowIfCancellationRequested();
            }

            return task;
        }

        private sealed class OnNextBuilder
        {
            private readonly IObserver<T> parent;

            private readonly T value;

            public OnNextBuilder(
                IObserver<T> parent, 
                T value)
            {
                this.parent = parent;
                this.value = value;
            }

            public void OnNext()
            {
                this.parent.OnNext(this.value);
            }
        }
    }
}
