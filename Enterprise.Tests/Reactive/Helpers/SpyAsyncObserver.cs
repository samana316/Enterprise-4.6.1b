using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Common.Runtime.ExceptionServices;
using Enterprise.Core.Linq;
using Enterprise.Core.Reactive;

namespace Enterprise.Tests.Reactive.Helpers
{
    internal static class SpyAsyncObserver
    {
        public static SpyAsyncObserver<TSource> CreateSpyAsyncObserver<TSource>(
            this IAsyncObservable<TSource> source,
            int delay = 1)
        {
            return new SpyAsyncObserver<TSource> { MillisecondsDelay = delay };
        }
    }

    internal sealed class SpyAsyncObserver<T> : IAsyncObserver<T>
    {
        private readonly IList<T> items = new List<T>();

        private readonly IList<Exception> errors = new List<Exception>();

        public SpyAsyncObserver()
        {
            this.MillisecondsDelay = 1;
        }

        public int MillisecondsDelay { get; set; }

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
            Trace.WriteLine(this.items.LastOrDefault(), "Current");
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

                if (this.MillisecondsDelay > 0)
                {
                    await Task.Delay(this.MillisecondsDelay, cancellationToken);
                }

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
