using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
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

        public void OnCompleted()
        {
            Trace.WriteLine("OnCompleted");
        }

        public void OnError(
            Exception error)
        {
            this.errors.Add(error);
            Trace.WriteLine(error, "OnError");
        }

        public void OnNext(
            T value)
        {
            this.items.Add(value);
            Console.WriteLine("OnNext: " + value);
        }

        public async Task OnNextAsync(
            T value,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await Task.Delay(5, cancellationToken);

            this.items.Add(value);
            await Console.Out.WriteLineAsync("OnNextAsync: " + value);
        }

        public void Reset()
        {
            this.items.Clear();
        }
    }
}
