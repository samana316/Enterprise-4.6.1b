using System;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Common;
using Enterprise.Core.Common.Runtime.CompilerServices;

namespace Enterprise.Core.Reactive
{
    internal sealed class AsyncSubscription : DisposableBase, IAsyncSubscription
    {
        private readonly Task task;

        private readonly CancellationTokenSource cancellationTokenSource;

        internal AsyncSubscription(
            Task task,
            CancellationToken cancellationToken)
        {
            this.task = task;

            if (cancellationToken.CanBeCanceled)
            {
                this.cancellationTokenSource = typeof(CancellationToken)
                    .GetField("m_source", BindingFlags.Instance | BindingFlags.NonPublic)
                    .GetValue(cancellationToken) as CancellationTokenSource;
            }
            else
            {
                this.cancellationTokenSource = new CancellationTokenSource();
            }
        }

        internal AsyncSubscription(
            Task task,
            CancellationTokenSource cancellationTokenSource)
        {
            this.task = task;
            this.cancellationTokenSource = cancellationTokenSource;
        }

        public IAwaiter GetAwaiter()
        {
            return new TaskAwaiterAdapter(this.task);
        }

        protected override void Dispose(
            bool disposing)
        {
            if (this.cancellationTokenSource != null)
            {
                if (!this.cancellationTokenSource.IsCancellationRequested)
                {
                    this.cancellationTokenSource.Cancel();
                }

                this.cancellationTokenSource.Dispose();
            }

            base.Dispose(disposing);
        }

        private sealed class TaskAwaiterAdapter : IAwaiter
        {
            private readonly TaskAwaiter awaiter;

            public TaskAwaiterAdapter(
                Task task)
            {
                this.awaiter = task.GetAwaiter();
            }

            public bool IsCompleted
            {
                get
                {
                    return this.awaiter.IsCompleted;
                }
            }

            public void GetResult()
            {
                this.awaiter.GetResult();
            }

            public void OnCompleted(
                Action continuation)
            {
                this.awaiter.OnCompleted(continuation);
            }

            public void UnsafeOnCompleted(
                Action continuation)
            {
                this.awaiter.UnsafeOnCompleted(continuation);
            }
        }
    }
}
