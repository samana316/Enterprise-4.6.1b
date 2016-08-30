using System;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Common.Runtime.ExceptionServices;

namespace Enterprise.Core.Linq
{
    internal abstract class AsyncEnumerableBase<TSource> : AsyncIterator<TSource>
    {
        private Consumer _yield;
        private TSource _current;
        private Task _enumerationTask;
        private Exception _enumerationException;

        public AsyncEnumerableBase()
        {
            ClearState();
        }

        public override sealed TSource Current
        {
            get
            {
                if (_enumerationTask == null)
                    throw new InvalidOperationException("Call MoveNext() or MoveNextAsync() before accessing the Current item");
                return _current;
            }
        }

        public override sealed void Reset()
        {
            ClearState();

            base.Reset();
        }

        protected override void Dispose(
            bool disposing)
        {
            ClearState();

            base.Dispose(disposing);
        }

        protected abstract Task EnumerateAsync(IAsyncYield<TSource> yield, CancellationToken cancellationToken);

        protected override sealed Task<bool> DoMoveNextAsync(
            CancellationToken cancellationToken)
        {
            if (_enumerationException != null)
            {
                var tcs = new TaskCompletionSource<bool>();
                tcs.SetException(_enumerationException);
                return tcs.Task;
            }

            var moveNextTask = _yield.OnMoveNext(cancellationToken).ContinueWith(OnMoveNextComplete, _yield);
            if (_enumerationTask == null)
                _enumerationTask = this.WrapperEnumerateAsync(_yield, cancellationToken).ContinueWith(OnEnumerationComplete, _yield);
            return moveNextTask;
        }

        private void ClearState()
        {
            if (_yield != null)
                _yield.Finilize();

            _yield = new Consumer();
            _enumerationTask = null;
            _enumerationException = null;
        }

        private bool OnMoveNextComplete(
            Task<TSource> task,
            object state)
        {
            var yield = (Consumer)state;

            if (task.IsFaulted)
            {
                _enumerationException = task.Exception;
                _enumerationException.Rethrow();
            }
            else if (task.IsCanceled)
            {
                return false;
            }

            if (yield.IsComplete)
            {
                return false;
            }

            _current = task.Result;
            return true;
        }

        private static void OnEnumerationComplete(
            Task task,
            object state)
        {
            var yield = (Consumer)state;
            if (task.IsFaulted)
            {
                if (task.Exception is AsyncEnumerationCanceledException)
                {
                    yield.SetCanceled();
                }
                else {
                    yield.SetFailed(task.Exception);
                }
            }
            else if (task.IsCanceled)
            {
                yield.SetCanceled();
            }
            else {
                yield.SetComplete();
            }
        }

        private async Task WrapperEnumerateAsync(
            IAsyncYield<TSource> yield, 
            CancellationToken cancellationToken)
        {
            try
            {
                await this.EnumerateAsync(yield, cancellationToken);

                yield.Break();
            }
            catch (Exception exception)
            {
                _yield.SetFailed(exception);
            }
        }

        private sealed class AsyncEnumerationCanceledException : OperationCanceledException { }

        private sealed class Consumer : IAsyncYield<TSource>
        {
            private TaskCompletionSource<bool> _resumeTCS;
            private TaskCompletionSource<TSource> _yieldTCS = new TaskCompletionSource<TSource>();

            public CancellationToken CancellationToken { get; private set; }

            public void Break()
            {
                if (!IsComplete)
                {
                    SetCanceled();
                    //throw new AsyncEnumerationCanceledException();
                }
            }

            public Task ReturnAsync(
                TSource value,
                CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();

                _resumeTCS = new TaskCompletionSource<bool>();
                _yieldTCS.TrySetResult(value);
                return _resumeTCS.Task;
            }

            internal void SetComplete()
            {
                _yieldTCS.TrySetCanceled();
                IsComplete = true;
            }

            internal void SetCanceled()
            {
                SetComplete();
            }

            internal void SetFailed(Exception ex)
            {
                var x = _yieldTCS.TrySetException(ex);
                IsComplete = true;
            }

            internal Task<TSource> OnMoveNext(
                CancellationToken cancellationToken)
            {
                if (!IsComplete)
                {
                    _yieldTCS = new TaskCompletionSource<TSource>();
                    CancellationToken = cancellationToken;

                    if (_resumeTCS != null)
                        _resumeTCS.SetResult(true);
                }
                return _yieldTCS.Task;
            }

            internal void Finilize()
            {
                SetCanceled();
            }

            internal bool IsComplete { get; set; }
        }
    }
}
