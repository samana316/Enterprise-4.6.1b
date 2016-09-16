using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Common.Runtime.ExceptionServices;
using Enterprise.Core.Linq;
using Enterprise.Core.Reactive;

namespace Enterprise.Tests.Reactive.Helpers
{
    internal sealed class SpyAsyncObserver<T> : IAsyncObserver<T>
    {
        private readonly IList<T> items = new List<T>();

        private readonly IList<Exception> errors = new List<Exception>();

        public IAsyncEnumerable<T> Items
        {
            get { return this.items.AsAsyncEnumerable(); }
        }

        public AggregateException Error
        {
            get { return new AggregateException(this.errors); }
        }

        public bool IsCompleted { get; private set; }

        public void OnCompleted()
        {
            if (this.IsCompleted)
            {
                return;
            }

            this.IsCompleted = true;
            Trace.WriteLine("OnCompleted");
        }

        public void OnError(
            Exception error)
        {
            if (this.IsCompleted)
            {
                return;
            }

            this.errors.Add(error);
            Trace.WriteLine(error, "OnError");
        }

        public void OnNext(
            T value)
        {
            throw new NotSupportedException();
        }

        public async Task OnNextAsync(
            T value,
            CancellationToken cancellationToken)
        {
            if (this.IsCompleted)
            {
                return;
            }

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                await Task.Delay(1, cancellationToken);

                this.items.Add(value);
                await Console.Out.WriteLineAsync("OnNextAsync: " + value);
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception, "OnError");

                exception.Rethrow();
            }
        }

        public void Reset()
        {
            this.items.Clear();
            this.errors.Clear();
            this.IsCompleted = false;
        }
    }
}
