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
            return this.task.FromTask().GetAwaiter();
        }

        protected override void Dispose(
            bool disposing)
        {
            if (disposing)
            {
                if (this.cancellationTokenSource != null)
                {
                    if (!this.cancellationTokenSource.IsCancellationRequested)
                    {
                        this.cancellationTokenSource.Cancel();
                    }

                    this.cancellationTokenSource.Dispose();
                }
            }

            base.Dispose(disposing);
        }
    }
}
